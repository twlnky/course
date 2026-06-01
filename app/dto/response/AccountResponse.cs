using CourseBank.model;

namespace CourseBank.dto.response;

public class AccountResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public CurrencyCode Currency { get; set; }
    public decimal Balance { get; set; }
    public bool IsBlocked { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal? CreditLimit { get; set; }
    public decimal? DebtBalance { get; set; }
    public decimal? InterestRate { get; set; }
    public DateTime? NextPaymentDate { get; set; }
    public int? CreditApplicationId { get; set; }
}
