using CourseBank.model;

namespace CourseBank.dto.request;

public class CreateTransactionRequest
{
    public int? FromAccountId { get; set; }
    public int? ToAccountId { get; set; }
    public decimal Amount { get; set; }
    public CurrencyCode Currency { get; set; }
    public TransactionType TransactionType { get; set; }
    public string? Description { get; set; }
}
