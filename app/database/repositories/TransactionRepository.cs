using CourseBank.interfaces;
using CourseBank.model;
using Microsoft.EntityFrameworkCore;

namespace CourseBank.database.repositories;

public class TransactionRepository(CourseDbContext db) : ITransactionRepository
{
    public IEnumerable<Transaction> GetByAccountId(int accountId) =>
        db.Transactions.AsNoTracking()
            .Where(t => t.FromAccountId == accountId || t.ToAccountId == accountId)
            .OrderByDescending(t => t.CreatedAt)
            .ToList();

    public Transaction? GetById(int id) =>
        db.Transactions.AsNoTracking().FirstOrDefault(t => t.Id == id);

    public Transaction? GetByIdForUpdate(int id) =>
        db.Transactions.FirstOrDefault(t => t.Id == id);

    public void Add(Transaction transaction) => db.Transactions.Add(transaction);

    public void Update(Transaction transaction) => db.Transactions.Update(transaction);
}
