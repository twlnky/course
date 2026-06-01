using CourseBank.interfaces;
using CourseBank.model;
using Microsoft.EntityFrameworkCore;

namespace CourseBank.database.repositories;

public class UserRepository(CourseDbContext db) : IUserRepository
{
    public IEnumerable<User> GetAll() =>
        db.Users.AsNoTracking().OrderBy(u => u.Id).ToList();

    public User? GetById(int id) =>
        db.Users.AsNoTracking().FirstOrDefault(u => u.Id == id);

    public User? GetByLogin(string login) =>
        db.Users.FirstOrDefault(u => u.Login == login);

    public bool LoginExists(string login) =>
        db.Users.IgnoreQueryFilters().Any(u => u.Login == login);

    public void Add(User user) => db.Users.Add(user);

    public void Update(User user) => db.Users.Update(user);

    public User? GetByIdForUpdate(int id) =>
        db.Users.FirstOrDefault(u => u.Id == id);

    public IEnumerable<Account> GetAccountsForUserSoftDelete(int userId) =>
        db.Accounts.IgnoreQueryFilters()
            .Where(a => a.UserId == userId && !a.IsDeleted)
            .ToList();
}
