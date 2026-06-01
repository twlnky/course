namespace CourseBank.dto.request;

public class ExecuteByTemplateRequest
{
    public int TemplateId { get; set; }
    public int FromAccountId { get; set; }
    public decimal? AmountOverride { get; set; }
}
