namespace CourseBank.dto.response;

public class OperationResultResponse
{
    public TransactionResponse Transaction { get; set; } = null!;
    public AccountBalanceResponse? FromAccountBalance { get; set; }
    public AccountBalanceResponse? ToAccountBalance { get; set; }
}
