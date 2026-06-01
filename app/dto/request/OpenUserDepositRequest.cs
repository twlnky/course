namespace CourseBank.dto.request;

public class OpenUserDepositRequest
{
    public int UserId { get; set; }
    public int DepositProductId { get; set; }
    public int FromAccountId { get; set; }
    public decimal Amount { get; set; }
}
