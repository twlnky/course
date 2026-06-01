using CourseBank.model;

namespace CourseBank.interfaces;

public interface IAccountRepository
{
    IEnumerable<Account> GetByUserId(int userId);
    Account? GetById(int id);
    Account? GetByIdForUpdate(int id);
    bool AccountNumberExists(string accountNumber);
    void Add(Account account);
    void Update(Account account);
}
