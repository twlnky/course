namespace CourseBank.model;

public class DepositProduct
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal MinAmount { get; set; }
    public decimal InterestRatePerYear { get; set; }
    public int TermDays { get; set; }
    public decimal EarlyWithdrawalPenalty { get; set; }
}
