using CourseBank.model;

namespace CourseBank.dto.request;

public class UpdatePaymentRecipientRequest
{
    public string? Name { get; set; }
    public string? TaxId { get; set; }
    public string? BankAccount { get; set; }
    public PaymentCategory? Category { get; set; }
}
