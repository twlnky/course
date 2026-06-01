using CourseBank.dto.request;
using CourseBank.dto.response;

namespace CourseBank.interfaces;

public interface IPaymentService
{
    OperationResultResponse ExecutePayment(ExecutePaymentRequest request);
    OperationResultResponse ExecuteByTemplate(ExecuteByTemplateRequest request);
}
