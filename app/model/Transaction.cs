namespace CourseBank.model;

public class Transaction
{
    public int Id { get; set; }
    public int? FromAccountId { get; set; }
    public int? ToAccountId { get; set; }
    public decimal Amount { get; set; }
    public CurrencyCode Currency { get; set; }
    public TransactionType TransactionType { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Description { get; set; }
    public decimal? CreditedAmount { get; set; }
    public CurrencyCode? CreditedCurrency { get; set; }
    public decimal? ExchangeRate { get; set; }

    public Account? FromAccount { get; set; }
    public Account? ToAccount { get; set; }
}
