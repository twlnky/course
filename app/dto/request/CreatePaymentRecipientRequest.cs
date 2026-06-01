using CourseBank.model;

namespace CourseBank.dto.request;

public class CreatePaymentRecipientRequest
{
    public string Name { get; set; } = string.Empty;
    public string? TaxId { get; set; }
    public string BankAccount { get; set; } = string.Empty;
    public PaymentCategory Category { get; set; }
}
