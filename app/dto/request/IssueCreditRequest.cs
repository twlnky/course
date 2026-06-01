using CourseBank.model;

namespace CourseBank.dto.request;

public class IssueCreditRequest
{
    public int ApplicationId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public CurrencyCode Currency { get; set; } = CurrencyCode.RUB;
}
