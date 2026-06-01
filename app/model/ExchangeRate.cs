namespace CourseBank.model;

public class ExchangeRate
{
    public CurrencyCode FromCurrency { get; set; }
    public CurrencyCode ToCurrency { get; set; }
    public decimal Rate { get; set; }
    public DateTime UpdatedAt { get; set; }
}
