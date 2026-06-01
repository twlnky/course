using CourseBank.interfaces;
using CourseBank.model;
using Microsoft.EntityFrameworkCore;

namespace CourseBank.database.repositories;

public class UserDepositRepository(CourseDbContext db) : IUserDepositRepository
{
    public IEnumerable<UserDeposit> GetByUserId(int userId) =>
        db.UserDeposits.AsNoTracking()
            .Include(d => d.DepositProduct)
            .Where(d => d.UserId == userId)
            .OrderByDescending(d => d.StartDate)
            .ToList();

    public UserDeposit? GetById(int id) =>
        db.UserDeposits.AsNoTracking()
            .Include(d => d.DepositProduct)
            .FirstOrDefault(d => d.Id == id);

    public UserDeposit? GetByIdForUpdate(int id) =>
        db.UserDeposits.Include(d => d.DepositProduct).FirstOrDefault(d => d.Id == id);

    public bool HasActiveDeposit(int userId, int productId) =>
        db.UserDeposits.Any(d => d.UserId == userId && d.DepositProductId == productId && !d.IsWithdrawn);

    public void Add(UserDeposit deposit) => db.UserDeposits.Add(deposit);

    public void Update(UserDeposit deposit) => db.UserDeposits.Update(deposit);
}
