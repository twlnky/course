using CourseBank.interfaces;
using CourseBank.model;
using Microsoft.EntityFrameworkCore;

namespace CourseBank.database.repositories;

public class AccountRepository(CourseDbContext db) : IAccountRepository
{
    public IEnumerable<Account> GetByUserId(int userId) =>
        db.Accounts.AsNoTracking().Where(a => a.UserId == userId).OrderBy(a => a.Id).ToList();

    public Account? GetById(int id) =>
        db.Accounts.AsNoTracking().FirstOrDefault(a => a.Id == id);

    public Account? GetByIdForUpdate(int id) =>
        db.Accounts.FirstOrDefault(a => a.Id == id);

    public bool AccountNumberExists(string accountNumber) =>
        db.Accounts.IgnoreQueryFilters().Any(a => a.AccountNumber == accountNumber);

    public void Add(Account account) => db.Accounts.Add(account);

    public void Update(Account account) => db.Accounts.Update(account);
}
