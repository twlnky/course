namespace CourseBank.dto.request;

public class UpdateAccountRequest
{
    public bool? IsBlocked { get; set; }
    public decimal? CreditLimit { get; set; }
    public decimal? InterestRate { get; set; }
    public DateTime? NextPaymentDate { get; set; }
}
