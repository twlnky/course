using CourseBank.dto.request;
using CourseBank.dto.response;
using CourseBank.interfaces;
using CourseBank.model;
using CourseBank.services;

namespace CourseBank.services;

public class TransactionService(
    IAccountRepository accountRepository,
    ITransactionRepository transactionRepository,
    ICreditApplicationRepository creditApplicationRepository,
    TransactionSettlement settlement,
    IUnitOfWork unitOfWork) : ITransactionService
{
    public IEnumerable<TransactionResponse> GetByAccountId(int accountId) =>
        transactionRepository.GetByAccountId(accountId).Select(EntityMapper.ToTransactionResponse);

    public TransactionResponse? GetById(int transactionId)
    {
        var transaction = transactionRepository.GetById(transactionId);
        return transaction == null ? null : EntityMapper.ToTransactionResponse(transaction);
    }

    public TransactionResponse Create(CreateTransactionRequest request)
    {
        AccountGuard.EnsurePositiveAmount(request.Amount);

        var transaction = new Transaction
        {
            FromAccountId = request.FromAccountId,
            ToAccountId = request.ToAccountId,
            Amount = request.Amount,
            Currency = request.Currency,
            TransactionType = request.TransactionType,
            Status = TransactionStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            Description = request.Description
        };

        transactionRepository.Add(transaction);
        unitOfWork.SaveChanges();
        return EntityMapper.ToTransactionResponse(transaction);
    }

    public TransactionResponse? UpdateStatus(int transactionId, UpdateTransactionStatusRequest request)
    {
        var transaction = transactionRepository.GetByIdForUpdate(transactionId);
        if (transaction == null) return null;

        var oldStatus = transaction.Status;
        var newStatus = request.Status;

        if (oldStatus == TransactionStatus.Pending && newStatus == TransactionStatus.Completed)
        {
            settlement.SettlePending(transaction);
        }
        else if (oldStatus == TransactionStatus.Completed && newStatus == TransactionStatus.Canceled)
        {
            settlement.ReverseSettled(transaction);
        }
        else if (oldStatus == TransactionStatus.Pending &&
                 (newStatus == TransactionStatus.Canceled || newStatus == TransactionStatus.Failed))
        {
            transaction.Status = newStatus;
        }
        else
        {
            throw new AppException($"Недопустимый переход статуса: {oldStatus} → {newStatus}");
        }

        transactionRepository.Update(transaction);
        unitOfWork.SaveChanges();
        return EntityMapper.ToTransactionResponse(transaction);
    }

    public OperationResultResponse Transfer(TransferRequest request)
    {
        AccountGuard.EnsurePositiveAmount(request.Amount);

        if (request.FromAccountId == request.ToAccountId)
            throw new AppException("Нельзя переводить на тот же счёт");

        var from = AccountGuard.RequireAccount(accountRepository.GetById(request.FromAccountId), "From account");
        var to = AccountGuard.RequireAccount(accountRepository.GetById(request.ToAccountId), "To account");

        var transaction = new Transaction
        {
            FromAccountId = request.FromAccountId,
            ToAccountId = request.ToAccountId,
            Amount = request.Amount,
            Currency = from.Currency,
            TransactionType = TransactionType.Transfer,
            Status = TransactionStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            Description = request.Description
        };

        transactionRepository.Add(transaction);
        settlement.SettlePending(transaction);
        unitOfWork.SaveChanges();

        return BuildOperationResult(transaction, request.FromAccountId, request.ToAccountId);
    }

    public OperationResultResponse RepayCredit(CreditRepaymentRequest request)
    {
        AccountGuard.EnsurePositiveAmount(request.Amount);

        var from = AccountGuard.RequireAccount(accountRepository.GetById(request.FromAccountId), "From account");
        var creditAccount = AccountGuard.RequireAccount(accountRepository.GetById(request.CreditAccountId), "Credit account");

        if (creditAccount.AccountType != AccountType.Credit)
            throw new AppException("Целевой счёт должен быть кредитным.");

        var transaction = new Transaction
        {
            FromAccountId = request.FromAccountId,
            ToAccountId = request.CreditAccountId,
            Amount = request.Amount,
            Currency = from.Currency,
            TransactionType = TransactionType.CreditRepayment,
            Status = TransactionStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            Description = request.Description
        };

        transactionRepository.Add(transaction);
        settlement.SettlePending(transaction);

        var updatedCredit = accountRepository.GetByIdForUpdate(request.CreditAccountId)!;
        if ((updatedCredit.DebtBalance ?? 0) == 0 && updatedCredit.CreditApplicationId.HasValue)
        {
            var application = creditApplicationRepository.GetByIdForUpdate(updatedCredit.CreditApplicationId.Value);
            if (application != null && application.Status == CreditApplicationStatus.Issued)
            {
                application.Status = CreditApplicationStatus.Closed;
                creditApplicationRepository.Update(application);
            }
        }

        unitOfWork.SaveChanges();
        return BuildOperationResult(transaction, request.FromAccountId, request.CreditAccountId);
    }

    private OperationResultResponse BuildOperationResult(Transaction transaction, int fromAccountId, int toAccountId)
    {
        var fromBalance = accountRepository.GetById(fromAccountId);
        var toBalance = accountRepository.GetById(toAccountId);

        return new OperationResultResponse
        {
            Transaction = EntityMapper.ToTransactionResponse(transaction),
            FromAccountBalance = fromBalance == null ? null : EntityMapper.ToAccountBalanceResponse(fromBalance),
            ToAccountBalance = toBalance == null ? null : EntityMapper.ToAccountBalanceResponse(toBalance)
        };
    }
}
