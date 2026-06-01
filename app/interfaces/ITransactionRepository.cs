using CourseBank.model;

namespace CourseBank.interfaces;

public interface ITransactionRepository
{
    IEnumerable<Transaction> GetByAccountId(int accountId);
    Transaction? GetById(int id);
    Transaction? GetByIdForUpdate(int id);
    void Add(Transaction transaction);
    void Update(Transaction transaction);
}
