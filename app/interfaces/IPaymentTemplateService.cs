using CourseBank.dto.request;
using CourseBank.dto.response;

namespace CourseBank.interfaces;

public interface IPaymentTemplateService
{
    IEnumerable<PaymentTemplateResponse> GetByUserId(int userId);
    PaymentTemplateResponse? GetById(int templateId);
    PaymentTemplateResponse Create(CreatePaymentTemplateRequest request);
    PaymentTemplateResponse? Update(int templateId, UpdatePaymentTemplateRequest request);
    bool Delete(int templateId);
}
