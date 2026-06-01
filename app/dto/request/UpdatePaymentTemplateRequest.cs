namespace CourseBank.dto.request;

public class UpdatePaymentTemplateRequest
{
    public decimal? Amount { get; set; }
    public string? Nickname { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsScheduled { get; set; }
    public DateTime? NextRunDate { get; set; }
    public int? PeriodDays { get; set; }
}
