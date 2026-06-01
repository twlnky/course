using CourseBank.dto.request;
using CourseBank.dto.response;

namespace CourseBank.interfaces;

public interface IAccountService
{
    IEnumerable<AccountResponse> GetByUserId(int userId);
    AccountResponse? GetById(int accountId);
    AccountBalanceResponse? GetBalance(int accountId);
    AccountResponse OpenAccount(OpenAccountRequest request);
    AccountResponse? Update(int accountId, UpdateAccountRequest request);
    AccountResponse? SetBlocked(int accountId, bool isBlocked);
    bool SoftDelete(int accountId);
}
