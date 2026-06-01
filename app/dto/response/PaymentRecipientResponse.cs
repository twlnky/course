using CourseBank.model;

namespace CourseBank.dto.response;

public class PaymentRecipientResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? TaxId { get; set; }
    public string BankAccount { get; set; } = string.Empty;
    public PaymentCategory Category { get; set; }
}
