using CourseBank.interfaces;
using CourseBank.model;

namespace CourseBank.services;

public class TransactionSettlement(
    IAccountRepository accountRepository,
    CurrencyConverter currencyConverter)
{
    public void SettlePending(Transaction transaction)
    {
        switch (transaction.TransactionType)
        {
            case TransactionType.Transfer:
                SettleTransfer(transaction);
                break;
            case TransactionType.Payment:
            case TransactionType.Fee:
                SettleDebitOnly(transaction);
                break;
            case TransactionType.DepositInterest:
                SettleCreditOnly(transaction);
                break;
            case TransactionType.CreditRepayment:
                SettleCreditRepayment(transaction);
                break;
            default:
                throw new AppException($"Неподдерживаемый тип транзакции {transaction.TransactionType}");
        }

        transaction.Status = TransactionStatus.Completed;
        transaction.CompletedAt = DateTime.UtcNow;
    }

    public void ReverseSettled(Transaction transaction)
    {
        switch (transaction.TransactionType)
        {
            case TransactionType.Transfer:
                ReverseTransfer(transaction);
                break;
            case TransactionType.Payment:
            case TransactionType.Fee:
                ReverseDebitOnly(transaction);
                break;
            case TransactionType.DepositInterest:
                ReverseCreditOnly(transaction);
                break;
            case TransactionType.CreditRepayment:
                ReverseCreditRepayment(transaction);
                break;
            default:
                throw new AppException($"Неподдерживаемый тип транзакции для отката: {transaction.TransactionType}");
        }

        transaction.Status = TransactionStatus.Canceled;
        transaction.CompletedAt = null;
    }

    private void SettleTransfer(Transaction transaction)
    {
        var from = AccountGuard.RequireAccount(
            accountRepository.GetByIdForUpdate(transaction.FromAccountId!.Value), "From account");
        var to = AccountGuard.RequireAccount(
            accountRepository.GetByIdForUpdate(transaction.ToAccountId!.Value), "To account");

        AccountGuard.EnsureNotBlocked(from);
        AccountGuard.EnsureNotBlocked(to);
        AccountGuard.EnsurePositiveAmount(transaction.Amount);
        AccountGuard.EnsureCurrencyMatch(from, transaction.Currency, "From");

        AccountGuard.EnsureSufficientFunds(from, transaction.Amount);
        from.Balance -= transaction.Amount;

        if (from.Currency == to.Currency)
        {
            to.Balance += transaction.Amount;
            transaction.CreditedAmount = transaction.Amount;
            transaction.CreditedCurrency = to.Currency;
            transaction.ExchangeRate = 1m;
        }
        else
        {
            var credited = currencyConverter.Convert(transaction.Amount, from.Currency, to.Currency, out var rate);
            to.Balance += credited;
            transaction.CreditedAmount = credited;
            transaction.CreditedCurrency = to.Currency;
            transaction.ExchangeRate = rate;
        }

        accountRepository.Update(from);
        accountRepository.Update(to);
    }

    private void ReverseTransfer(Transaction transaction)
    {
        var from = AccountGuard.RequireAccount(
            accountRepository.GetByIdForUpdate(transaction.FromAccountId!.Value), "From account");
        var to = AccountGuard.RequireAccount(
            accountRepository.GetByIdForUpdate(transaction.ToAccountId!.Value), "To account");

        from.Balance += transaction.Amount;
        var creditAmount = transaction.CreditedAmount ?? transaction.Amount;
        to.Balance -= creditAmount;

        accountRepository.Update(from);
        accountRepository.Update(to);
    }

    private void SettleDebitOnly(Transaction transaction)
    {
        var from = AccountGuard.RequireAccount(
            accountRepository.GetByIdForUpdate(transaction.FromAccountId!.Value), "From account");

        AccountGuard.EnsureNotBlocked(from);
        AccountGuard.EnsurePositiveAmount(transaction.Amount);
        AccountGuard.EnsureCurrencyMatch(from, transaction.Currency, "From");
        AccountGuard.EnsureSufficientFunds(from, transaction.Amount);

        from.Balance -= transaction.Amount;
        accountRepository.Update(from);
    }

    private void ReverseDebitOnly(Transaction transaction)
    {
        var from = AccountGuard.RequireAccount(
            accountRepository.GetByIdForUpdate(transaction.FromAccountId!.Value), "From account");
        from.Balance += transaction.Amount;
        accountRepository.Update(from);
    }

    private void SettleCreditOnly(Transaction transaction)
    {
        var to = AccountGuard.RequireAccount(
            accountRepository.GetByIdForUpdate(transaction.ToAccountId!.Value), "To account");

        AccountGuard.EnsureNotBlocked(to);
        AccountGuard.EnsurePositiveAmount(transaction.Amount);

        var creditAmount = transaction.CreditedAmount ?? transaction.Amount;
        if (transaction.CreditedCurrency.HasValue)
            AccountGuard.EnsureCurrencyMatch(to, transaction.CreditedCurrency.Value, "To");
        else
            AccountGuard.EnsureCurrencyMatch(to, transaction.Currency, "To");

        to.Balance += creditAmount;
        accountRepository.Update(to);
    }

    private void ReverseCreditOnly(Transaction transaction)
    {
        var to = AccountGuard.RequireAccount(
            accountRepository.GetByIdForUpdate(transaction.ToAccountId!.Value), "To account");
        var creditAmount = transaction.CreditedAmount ?? transaction.Amount;
        to.Balance -= creditAmount;
        accountRepository.Update(to);
    }

    private void SettleCreditRepayment(Transaction transaction)
    {
        var from = AccountGuard.RequireAccount(
            accountRepository.GetByIdForUpdate(transaction.FromAccountId!.Value), "From account");
        var creditAccount = AccountGuard.RequireAccount(
            accountRepository.GetByIdForUpdate(transaction.ToAccountId!.Value), "Credit account");

        if (creditAccount.AccountType != AccountType.Credit)
            throw new AppException("Целевой счёт должен быть кредитным.");

        AccountGuard.EnsureNotBlocked(from);
        AccountGuard.EnsureNotBlocked(creditAccount);
        AccountGuard.EnsurePositiveAmount(transaction.Amount);
        AccountGuard.EnsureCurrencyMatch(from, transaction.Currency, "From");
        AccountGuard.EnsureCurrencyMatch(creditAccount, transaction.Currency, "Credit");

        var debt = creditAccount.DebtBalance ?? 0;
        var actualAmount = Math.Min(transaction.Amount, debt);
        if (actualAmount <= 0)
            throw new AppException("Кредитный долг уже погашен.");

        transaction.Amount = actualAmount;
        AccountGuard.EnsureSufficientFunds(from, actualAmount);

        from.Balance -= actualAmount;
        creditAccount.DebtBalance = debt - actualAmount;
        creditAccount.Balance += actualAmount;

        accountRepository.Update(from);
        accountRepository.Update(creditAccount);
    }

    private void ReverseCreditRepayment(Transaction transaction)
    {
        var from = AccountGuard.RequireAccount(
            accountRepository.GetByIdForUpdate(transaction.FromAccountId!.Value), "From account");
        var creditAccount = AccountGuard.RequireAccount(
            accountRepository.GetByIdForUpdate(transaction.ToAccountId!.Value), "Credit account");

        from.Balance += transaction.Amount;
        creditAccount.DebtBalance = (creditAccount.DebtBalance ?? 0) + transaction.Amount;
        creditAccount.Balance -= transaction.Amount;

        accountRepository.Update(from);
        accountRepository.Update(creditAccount);
    }
}
