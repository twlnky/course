using CourseBank.dto.request;
using CourseBank.dto.response;

namespace CourseBank.interfaces;

public interface ICreditApplicationService
{
    IEnumerable<CreditApplicationResponse> GetByUserId(int userId);
    CreditApplicationResponse? GetById(int applicationId);
    CreditApplicationResponse Submit(SubmitCreditApplicationRequest request);
    CreditApplicationResponse? Review(int applicationId, ReviewCreditApplicationRequest request);
    AccountResponse IssueCredit(IssueCreditRequest request);
}
