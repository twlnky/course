using CourseBank.model;

namespace CourseBank.services;

public static class AccountGuard
{
    public static Account RequireAccount(Account? account, string label = "Счёт")
    {
        if (account == null)
            throw new AppException($"{label} не найден.", StatusCodes.Status404NotFound);
        return account;
    }

    public static void EnsureNotBlocked(Account account)
    {
        if (account.IsBlocked)
            throw new AppException($"Счёт {account.AccountNumber} заблокирован.");
    }

    public static void EnsurePositiveAmount(decimal amount)
    {
        if (amount <= 0)
            throw new AppException("Сумма должна быть больше нуля.");
    }

    public static void EnsureCurrencyMatch(Account account, CurrencyCode currency, string side)
    {
        if (account.Currency != currency)
            throw new AppException($"Валюта {side}-счёта не совпадает с валютой операции.");
    }

    public static void EnsureSufficientFunds(Account account, decimal amount)
    {
        if (account.AccountType == AccountType.Credit)
        {
            var available = (account.CreditLimit ?? 0) + account.Balance;
            if (available < amount)
                throw new AppException("Недостаточно кредитного лимита.");
            return;
        }

        if (account.Balance < amount)
            throw new AppException("Недостаточно средств на счёте.");
    }
}
