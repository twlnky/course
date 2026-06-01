namespace CourseBank.model;

public class UserDeposit
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int DepositProductId { get; set; }
    public decimal Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime MaturityDate { get; set; }
    public bool IsWithdrawn { get; set; }
    public decimal AccruedInterest { get; set; }

    public User User { get; set; } = null!;
    public DepositProduct DepositProduct { get; set; } = null!;
}
