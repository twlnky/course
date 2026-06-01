namespace CourseBank.dto.request;

public class CreditRepaymentRequest
{
    public int FromAccountId { get; set; }
    public int CreditAccountId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
}
