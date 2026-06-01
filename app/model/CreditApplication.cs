namespace CourseBank.model;

public class CreditApplication
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal AmountRequested { get; set; }
    public int TermMonths { get; set; }
    public decimal InterestRate { get; set; }
    public decimal MonthlyPayment { get; set; }
    public CreditApplicationStatus Status { get; set; } = CreditApplicationStatus.Pending;
    public DateTime AppliedAt { get; set; }
    public DateTime? DecisionAt { get; set; }
    public string? OperatorComment { get; set; }

    public User User { get; set; } = null!;
    public Account? IssuedAccount { get; set; }
}
