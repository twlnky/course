using CourseBank.model;

namespace CourseBank.dto.request;

public class ReviewCreditApplicationRequest
{
    public CreditApplicationStatus Status { get; set; }
    public string? OperatorComment { get; set; }
}
