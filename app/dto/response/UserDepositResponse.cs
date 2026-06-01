namespace CourseBank.dto.response;

public class UserDepositResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int DepositProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime MaturityDate { get; set; }
    public bool IsWithdrawn { get; set; }
    public decimal AccruedInterest { get; set; }
}
