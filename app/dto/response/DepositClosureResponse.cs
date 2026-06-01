namespace CourseBank.dto.response;

public class DepositClosureResponse
{
    public UserDepositResponse Deposit { get; set; } = null!;
    public decimal AccruedInterest { get; set; }
    public decimal PenaltyAmount { get; set; }
    public decimal TotalPayout { get; set; }
    public TransactionResponse? InterestTransaction { get; set; }
}
