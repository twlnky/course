using CourseBank.model;

namespace CourseBank.dto.request;

public class OpenAccountRequest
{
    public int UserId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public CurrencyCode Currency { get; set; }
    public decimal? CreditLimit { get; set; }
    public decimal? InterestRate { get; set; }
}
