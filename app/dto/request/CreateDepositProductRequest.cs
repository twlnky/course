namespace CourseBank.dto.request;

public class CreateDepositProductRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal MinAmount { get; set; }
    public decimal InterestRatePerYear { get; set; }
    public int TermDays { get; set; }
    public decimal EarlyWithdrawalPenalty { get; set; }
}
