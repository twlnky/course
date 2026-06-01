using CourseBank.model;

namespace CourseBank.interfaces;

public interface IPaymentTemplateRepository
{
    IEnumerable<PaymentTemplate> GetByUserId(int userId);
    PaymentTemplate? GetById(int id);
    PaymentTemplate? GetByIdForUpdate(int id);
    void Add(PaymentTemplate template);
    void Update(PaymentTemplate template);
    void Delete(PaymentTemplate template);
    void DeactivateByUserId(int userId);
}
