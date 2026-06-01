using CourseBank.model;

namespace CourseBank.interfaces;

public interface IPaymentRecipientRepository
{
    IEnumerable<PaymentRecipient> GetAll(string? nameFilter);
    PaymentRecipient? GetById(int id);
    PaymentRecipient? GetByIdForUpdate(int id);
    void Add(PaymentRecipient recipient);
    void Update(PaymentRecipient recipient);
    void Delete(PaymentRecipient recipient);
}
