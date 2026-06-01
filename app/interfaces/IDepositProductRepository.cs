using CourseBank.model;

namespace CourseBank.interfaces;

public interface IDepositProductRepository
{
    IEnumerable<DepositProduct> GetAll();
    DepositProduct? GetById(int id);
    void Add(DepositProduct product);
}
