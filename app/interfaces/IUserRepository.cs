using CourseBank.model;

namespace CourseBank.interfaces;

public interface IUserRepository
{
    IEnumerable<User> GetAll();
    User? GetById(int id);
    User? GetByIdForUpdate(int id);
    User? GetByLogin(string login);
    bool LoginExists(string login);
    IEnumerable<Account> GetAccountsForUserSoftDelete(int userId);
    void Add(User user);
    void Update(User user);
}
