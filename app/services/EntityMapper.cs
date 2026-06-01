using CourseBank.dto.response;
using CourseBank.model;

namespace CourseBank.services;

public static class EntityMapper
{
    public static UserResponse ToUserResponse(User user) => new()
    {
        Id = user.Id,
        Login = user.Login,
        FullName = user.FullName,
        Role = user.Role,
        IsActive = user.IsActive,
        RegistrationDate = user.RegistrationDate,
        LastLoginDate = user.LastLoginDate
    };

    public static AccountResponse ToAccountResponse(Account account) => new()
    {
        Id = account.Id,
        UserId = account.UserId,
        AccountNumber = account.AccountNumber,
        AccountType = account.AccountType,
        Currency = account.Currency,
        Balance = account.Balance,
        IsBlocked = account.IsBlocked,
        CreatedAt = account.CreatedAt,
        CreditLimit = account.CreditLimit,
        DebtBalance = account.DebtBalance,
        InterestRate = account.InterestRate,
        NextPaymentDate = account.NextPaymentDate,
        CreditApplicationId = account.CreditApplicationId
    };

    public static AccountBalanceResponse ToAccountBalanceResponse(Account account) => new()
    {
        AccountId = account.Id,
        AccountNumber = account.AccountNumber,
        Currency = account.Currency,
        Balance = account.Balance,
        DebtBalance = account.DebtBalance
    };

    public static TransactionResponse ToTransactionResponse(Transaction transaction) => new()
    {
        Id = transaction.Id,
        FromAccountId = transaction.FromAccountId,
        ToAccountId = transaction.ToAccountId,
        Amount = transaction.Amount,
        Currency = transaction.Currency,
        TransactionType = transaction.TransactionType,
        Status = transaction.Status,
        CreatedAt = transaction.CreatedAt,
        CompletedAt = transaction.CompletedAt,
        Description = transaction.Description,
        CreditedAmount = transaction.CreditedAmount,
        CreditedCurrency = transaction.CreditedCurrency,
        ExchangeRate = transaction.ExchangeRate
    };

    public static CreditApplicationResponse ToCreditApplicationResponse(CreditApplication app) => new()
    {
        Id = app.Id,
        UserId = app.UserId,
        AmountRequested = app.AmountRequested,
        TermMonths = app.TermMonths,
        InterestRate = app.InterestRate,
        MonthlyPayment = app.MonthlyPayment,
        Status = app.Status,
        AppliedAt = app.AppliedAt,
        DecisionAt = app.DecisionAt,
        OperatorComment = app.OperatorComment
    };

    public static PaymentRecipientResponse ToPaymentRecipientResponse(PaymentRecipient recipient) => new()
    {
        Id = recipient.Id,
        Name = recipient.Name,
        TaxId = recipient.TaxId,
        BankAccount = recipient.BankAccount,
        Category = recipient.Category
    };

    public static PaymentTemplateResponse ToPaymentTemplateResponse(PaymentTemplate template) => new()
    {
        Id = template.Id,
        UserId = template.UserId,
        RecipientId = template.RecipientId,
        RecipientName = template.Recipient?.Name ?? string.Empty,
        Amount = template.Amount,
        Nickname = template.Nickname,
        IsActive = template.IsActive,
        IsScheduled = template.IsScheduled,
        NextRunDate = template.NextRunDate,
        PeriodDays = template.PeriodDays
    };

    public static DepositProductResponse ToDepositProductResponse(DepositProduct product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        MinAmount = product.MinAmount,
        InterestRatePerYear = product.InterestRatePerYear,
        TermDays = product.TermDays,
        EarlyWithdrawalPenalty = product.EarlyWithdrawalPenalty
    };

    public static UserDepositResponse ToUserDepositResponse(UserDeposit deposit) => new()
    {
        Id = deposit.Id,
        UserId = deposit.UserId,
        DepositProductId = deposit.DepositProductId,
        ProductName = deposit.DepositProduct?.Name ?? string.Empty,
        Amount = deposit.Amount,
        StartDate = deposit.StartDate,
        MaturityDate = deposit.MaturityDate,
        IsWithdrawn = deposit.IsWithdrawn,
        AccruedInterest = deposit.AccruedInterest
    };
}
