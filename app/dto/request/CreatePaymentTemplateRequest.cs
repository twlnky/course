namespace CourseBank.dto.request;

public class CreatePaymentTemplateRequest
{
    public int UserId { get; set; }
    public int RecipientId { get; set; }
    public decimal? Amount { get; set; }
    public string? Nickname { get; set; }
    public bool IsScheduled { get; set; }
    public DateTime? NextRunDate { get; set; }
    public int? PeriodDays { get; set; }
}
