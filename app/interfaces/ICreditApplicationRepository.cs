using CourseBank.model;

namespace CourseBank.interfaces;

public interface ICreditApplicationRepository
{
    IEnumerable<CreditApplication> GetByUserId(int userId);
    CreditApplication? GetById(int id);
    CreditApplication? GetByIdForUpdate(int id);
    bool HasActiveApplication(int userId);
    void Add(CreditApplication application);
    void Update(CreditApplication application);
}
