using CourseBank.dto.request;
using CourseBank.dto.response;
using CourseBank.interfaces;
using CourseBank.model;
using CourseBank.services;

namespace CourseBank.services;

public class DepositService(
    IUserRepository userRepository,
    IAccountRepository accountRepository,
    IDepositProductRepository productRepository,
    IUserDepositRepository userDepositRepository,
    ITransactionRepository transactionRepository,
    TransactionSettlement settlement,
    IUnitOfWork unitOfWork) : IDepositService
{
    public IEnumerable<DepositProductResponse> GetAllProducts() =>
        productRepository.GetAll().Select(EntityMapper.ToDepositProductResponse);

    public DepositProductResponse? GetProductById(int productId)
    {
        var product = productRepository.GetById(productId);
        return product == null ? null : EntityMapper.ToDepositProductResponse(product);
    }

    public DepositProductResponse CreateProduct(CreateDepositProductRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new AppException("Название продукта обязательно.");
        if (request.MinAmount <= 0 || request.TermDays <= 0)
            throw new AppException("Минимальная сумма и срок должны быть больше нуля.");

        var product = new DepositProduct
        {
            Name = request.Name,
            MinAmount = request.MinAmount,
            InterestRatePerYear = request.InterestRatePerYear,
            TermDays = request.TermDays,
            EarlyWithdrawalPenalty = request.EarlyWithdrawalPenalty
        };

        productRepository.Add(product);
        unitOfWork.SaveChanges();
        return EntityMapper.ToDepositProductResponse(product);
    }

    public IEnumerable<UserDepositResponse> GetUserDeposits(int userId) =>
        userDepositRepository.GetByUserId(userId).Select(EntityMapper.ToUserDepositResponse);

    public UserDepositResponse? GetUserDepositById(int userDepositId)
    {
        var deposit = userDepositRepository.GetById(userDepositId);
        return deposit == null ? null : EntityMapper.ToUserDepositResponse(deposit);
    }

    public UserDepositResponse OpenDeposit(OpenUserDepositRequest request)
    {
        AccountGuard.EnsurePositiveAmount(request.Amount);

        if (userRepository.GetById(request.UserId) == null)
            throw new AppException("Пользователь не найден.", StatusCodes.Status404NotFound);

        var product = productRepository.GetById(request.DepositProductId);
        if (product == null)
            throw new AppException("Продукт вклада не найден.", StatusCodes.Status404NotFound);

        if (request.Amount < product.MinAmount)
            throw new AppException($"Минимальная сумма вклада: {product.MinAmount}.");

        if (userDepositRepository.HasActiveDeposit(request.UserId, request.DepositProductId))
            throw new AppException("У пользователя уже есть активный вклад по этому продукту.", StatusCodes.Status409Conflict);

        var fromAccount = AccountGuard.RequireAccount(
            accountRepository.GetById(request.FromAccountId), "From account");
        AccountGuard.EnsureNotBlocked(fromAccount);
        AccountGuard.EnsureSufficientFunds(fromAccount, request.Amount);

        var feeTransaction = new Transaction
        {
            FromAccountId = request.FromAccountId,
            Amount = request.Amount,
            Currency = fromAccount.Currency,
            TransactionType = TransactionType.Fee,
            Status = TransactionStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            Description = $"Deposit opening fee for product {product.Name}"
        };

        transactionRepository.Add(feeTransaction);
        settlement.SettlePending(feeTransaction);

        var startDate = DateTime.UtcNow;
        var deposit = new UserDeposit
        {
            UserId = request.UserId,
            DepositProductId = request.DepositProductId,
            Amount = request.Amount,
            StartDate = startDate,
            MaturityDate = startDate.AddDays(product.TermDays),
            IsWithdrawn = false,
            AccruedInterest = 0
        };

        userDepositRepository.Add(deposit);
        unitOfWork.SaveChanges();

        var created = userDepositRepository.GetById(deposit.Id)!;
        return EntityMapper.ToUserDepositResponse(created);
    }

    public DepositClosureResponse EarlyWithdraw(EarlyWithdrawRequest request)
    {
        var deposit = userDepositRepository.GetByIdForUpdate(request.UserDepositId);
        if (deposit == null)
            throw new AppException("Вклад не найден.", StatusCodes.Status404NotFound);

        if (deposit.IsWithdrawn)
            throw new AppException("Вклад уже закрыт.");

        var toAccount = AccountGuard.RequireAccount(
            accountRepository.GetById(request.ToAccountId), "To account");
        AccountGuard.EnsureNotBlocked(toAccount);

        var product = deposit.DepositProduct;
        var daysHeld = Math.Max(0, (DateTime.UtcNow - deposit.StartDate).Days);
        var interest = CreditCalculator.CalculateAccruedInterest(
            deposit.Amount, product.InterestRatePerYear, daysHeld);

        var isEarly = DateTime.UtcNow < deposit.MaturityDate;
        var penalty = isEarly
            ? Math.Round(deposit.Amount * product.EarlyWithdrawalPenalty / 100m, 2)
            : 0m;

        var totalPayout = deposit.Amount + interest - penalty;
        if (totalPayout < 0)
            throw new AppException("Итоговая выплата не может быть отрицательной.");

        Transaction? interestTransaction = null;
        if (totalPayout > 0)
        {
            interestTransaction = new Transaction
            {
                ToAccountId = request.ToAccountId,
                Amount = totalPayout,
                Currency = toAccount.Currency,
                CreditedAmount = totalPayout,
                CreditedCurrency = toAccount.Currency,
                TransactionType = TransactionType.DepositInterest,
                Status = TransactionStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                Description = isEarly
                    ? $"Early deposit closure with penalty {penalty}"
                    : "Deposit maturity payout"
            };

            transactionRepository.Add(interestTransaction);
            settlement.SettlePending(interestTransaction);
        }

        deposit.AccruedInterest = interest;
        deposit.IsWithdrawn = true;
        userDepositRepository.Update(deposit);
        unitOfWork.SaveChanges();

        var updated = userDepositRepository.GetById(deposit.Id)!;
        return new DepositClosureResponse
        {
            Deposit = EntityMapper.ToUserDepositResponse(updated),
            AccruedInterest = interest,
            PenaltyAmount = penalty,
            TotalPayout = totalPayout,
            InterestTransaction = interestTransaction == null
                ? null
                : EntityMapper.ToTransactionResponse(interestTransaction)
        };
    }
}
