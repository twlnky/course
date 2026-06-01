using CourseBank.dto.request;
using CourseBank.dto.response;

namespace CourseBank.interfaces;

public interface IPaymentRecipientService
{
    IEnumerable<PaymentRecipientResponse> GetAll(string? name);
    PaymentRecipientResponse? GetById(int recipientId);
    PaymentRecipientResponse Create(CreatePaymentRecipientRequest request);
    PaymentRecipientResponse? Update(int recipientId, UpdatePaymentRecipientRequest request);
    bool Delete(int recipientId);
}
