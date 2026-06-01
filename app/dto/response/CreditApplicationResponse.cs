using CourseBank.model;

namespace CourseBank.dto.response;

public class CreditApplicationResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal AmountRequested { get; set; }
    public int TermMonths { get; set; }
    public decimal InterestRate { get; set; }
    public decimal MonthlyPayment { get; set; }
    public CreditApplicationStatus Status { get; set; }
    public DateTime AppliedAt { get; set; }
    public DateTime? DecisionAt { get; set; }
    public string? OperatorComment { get; set; }
    
    
}
