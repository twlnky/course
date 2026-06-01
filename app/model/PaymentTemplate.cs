namespace CourseBank.model;

public class PaymentTemplate
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RecipientId { get; set; }
    public decimal? Amount { get; set; }
    public string? Nickname { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsScheduled { get; set; }
    public DateTime? NextRunDate { get; set; }
    public int? PeriodDays { get; set; }

    public User User { get; set; } = null!;
    public PaymentRecipient Recipient { get; set; } = null!;
}
