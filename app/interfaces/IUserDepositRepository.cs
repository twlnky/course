using CourseBank.model;

namespace CourseBank.interfaces;

public interface IUserDepositRepository
{
    IEnumerable<UserDeposit> GetByUserId(int userId);
    UserDeposit? GetById(int id);
    UserDeposit? GetByIdForUpdate(int id);
    bool HasActiveDeposit(int userId, int productId);
    void Add(UserDeposit deposit);
    void Update(UserDeposit deposit);
}
