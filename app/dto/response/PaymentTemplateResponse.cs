namespace CourseBank.dto.response;

public class PaymentTemplateResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RecipientId { get; set; }
    public string RecipientName { get; set; } = string.Empty;
    public decimal? Amount { get; set; }
    public string? Nickname { get; set; }
    public bool IsActive { get; set; }
    public bool IsScheduled { get; set; }
    public DateTime? NextRunDate { get; set; }
    public int? PeriodDays { get; set; }
}
