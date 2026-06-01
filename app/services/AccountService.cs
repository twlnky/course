using CourseBank.dto.request;
using CourseBank.dto.response;
using CourseBank.interfaces;
using CourseBank.model;

namespace CourseBank.services;

public class AccountService(
    IUserRepository userRepository,
    IAccountRepository accountRepository,
    IUnitOfWork unitOfWork) : IAccountService
{
    public IEnumerable<AccountResponse> GetByUserId(int userId) =>
        accountRepository.GetByUserId(userId).Select(EntityMapper.ToAccountResponse);

    public AccountResponse? GetById(int accountId)
    {
        var account = accountRepository.GetById(accountId);
        return account == null ? null : EntityMapper.ToAccountResponse(account);
    }

    public AccountBalanceResponse? GetBalance(int accountId)
    {
        var account = accountRepository.GetById(accountId);
        return account == null ? null : EntityMapper.ToAccountBalanceResponse(account);
    }

    public AccountResponse OpenAccount(OpenAccountRequest request)
    {
        if (userRepository.GetById(request.UserId) == null)
            throw new AppException("Пользователь не найден.", StatusCodes.Status404NotFound);

        if (string.IsNullOrWhiteSpace(request.AccountNumber))
            throw new AppException("Номер счёта обязателен.");

        if (accountRepository.AccountNumberExists(request.AccountNumber))
            throw new AppException("Счёт с таким номером уже существует.", StatusCodes.Status409Conflict);

        if (request.AccountType == AccountType.Credit)
        {
            if (!request.CreditLimit.HasValue || request.CreditLimit <= 0)
                throw new AppException("Для кредитного счёта лимит должен быть больше нуля.");
        }
        else if (request.CreditLimit.HasValue)
        {
            throw new AppException("Кредитный лимит допустим только для кредитного счёта.");
        }

        var account = new Account
        {
            UserId = request.UserId,
            AccountNumber = request.AccountNumber,
            AccountType = request.AccountType,
            Currency = request.Currency,
            Balance = 0,
            IsBlocked = false,
            CreatedAt = DateTime.UtcNow,
            CreditLimit = request.AccountType == AccountType.Credit ? request.CreditLimit : null,
            DebtBalance = request.AccountType == AccountType.Credit ? 0 : null,
            InterestRate = request.InterestRate
        };

        accountRepository.Add(account);
        unitOfWork.SaveChanges();
        return EntityMapper.ToAccountResponse(account);
    }

    public AccountResponse? Update(int accountId, UpdateAccountRequest request)
    {
        var account = accountRepository.GetByIdForUpdate(accountId);
        if (account == null) return null;

        if (request.IsBlocked.HasValue)
            account.IsBlocked = request.IsBlocked.Value;
        if (request.CreditLimit.HasValue)
        {
            if (account.AccountType != AccountType.Credit)
                throw new AppException("Кредитный лимит допустим только для кредитного счёта.");
            account.CreditLimit = request.CreditLimit;
        }
        if (request.InterestRate.HasValue)
            account.InterestRate = request.InterestRate;
        if (request.NextPaymentDate.HasValue)
            account.NextPaymentDate = request.NextPaymentDate;

        accountRepository.Update(account);
        unitOfWork.SaveChanges();
        return EntityMapper.ToAccountResponse(account);
    }

    public AccountResponse? SetBlocked(int accountId, bool isBlocked)
    {
        var account = accountRepository.GetByIdForUpdate(accountId);
        if (account == null) return null;

        account.IsBlocked = isBlocked;
        accountRepository.Update(account);
        unitOfWork.SaveChanges();
        return EntityMapper.ToAccountResponse(account);
    }

    public bool SoftDelete(int accountId)
    {
        var account = accountRepository.GetByIdForUpdate(accountId);
        if (account == null) return false;

        if (account.Balance != 0)
            throw new AppException("Нельзя удалить счёт: баланс не равен нулю.");

        if (account.AccountType == AccountType.Credit && (account.DebtBalance ?? 0) != 0)
            throw new AppException("Нельзя удалить счёт: остался кредитный долг.");

        account.IsDeleted = true;
        account.DeletedAt = DateTime.UtcNow;
        accountRepository.Update(account);
        unitOfWork.SaveChanges();
        return true;
    }
}
