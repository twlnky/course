namespace CourseBank.dto.request;

public class ExecutePaymentRequest
{
    public int FromAccountId { get; set; }
    public int RecipientId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
}
