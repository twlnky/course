namespace CourseBank.dto.request;

public class SubmitCreditApplicationRequest
{
    public int UserId { get; set; }
    public decimal AmountRequested { get; set; }
    public int TermMonths { get; set; }
    public decimal InterestRate { get; set; }
}
