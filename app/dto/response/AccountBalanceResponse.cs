using CourseBank.model;

namespace CourseBank.dto.response;

public class AccountBalanceResponse
{
    public int AccountId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public CurrencyCode Currency { get; set; }
    public decimal Balance { get; set; }
    public decimal? DebtBalance { get; set; }
}
