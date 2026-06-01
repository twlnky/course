using CourseBank.dto.request;
using CourseBank.dto.response;
using CourseBank.interfaces;
using CourseBank.model;
using CourseBank.services;

namespace CourseBank.services;

public class PaymentService(
    IAccountRepository accountRepository,
    IPaymentRecipientRepository recipientRepository,
    IPaymentTemplateRepository templateRepository,
    ITransactionRepository transactionRepository,
    TransactionSettlement settlement,
    IUnitOfWork unitOfWork) : IPaymentService
{
    public OperationResultResponse ExecutePayment(ExecutePaymentRequest request)
    {
        AccountGuard.EnsurePositiveAmount(request.Amount);

        var from = AccountGuard.RequireAccount(accountRepository.GetById(request.FromAccountId), "From account");
        var recipient = recipientRepository.GetById(request.RecipientId);
        if (recipient == null)
            throw new AppException("Получатель платежа не найден.", StatusCodes.Status404NotFound);

        var transaction = new Transaction
        {
            FromAccountId = request.FromAccountId,
            Amount = request.Amount,
            Currency = from.Currency,
            TransactionType = TransactionType.Payment,
            Status = TransactionStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            Description = request.Description ?? $"Payment to {recipient.Name}"
        };

        transactionRepository.Add(transaction);
        settlement.SettlePending(transaction);
        unitOfWork.SaveChanges();

        var fromBalance = accountRepository.GetById(request.FromAccountId);
        return new OperationResultResponse
        {
            Transaction = EntityMapper.ToTransactionResponse(transaction),
            FromAccountBalance = fromBalance == null ? null : EntityMapper.ToAccountBalanceResponse(fromBalance)
        };
    }

    public OperationResultResponse ExecuteByTemplate(ExecuteByTemplateRequest request)
    {
        var template = templateRepository.GetByIdForUpdate(request.TemplateId);
        if (template == null)
            throw new AppException("Шаблон платежа не найден.", StatusCodes.Status404NotFound);

        if (!template.IsActive)
            throw new AppException("Шаблон платежа неактивен.");

        var amount = request.AmountOverride ?? template.Amount;
        if (!amount.HasValue || amount <= 0)
            throw new AppException("Сумма платежа обязательна.");

        var result = ExecutePayment(new ExecutePaymentRequest
        {
            FromAccountId = request.FromAccountId,
            RecipientId = template.RecipientId,
            Amount = amount.Value,
            Description = template.Nickname ?? $"Template payment #{template.Id}"
        });

        if (template.IsScheduled && template.PeriodDays.HasValue)
        {
            template.NextRunDate = (template.NextRunDate ?? DateTime.UtcNow).AddDays(template.PeriodDays.Value);
            templateRepository.Update(template);
            unitOfWork.SaveChanges();
        }

        return result;
    }
}
