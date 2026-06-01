namespace CourseBank.model;

public class User
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Client;
    public bool IsActive { get; set; } = true;
    public DateTime RegistrationDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<Account> Accounts { get; set; } = new List<Account>();
    public ICollection<PaymentTemplate> PaymentTemplates { get; set; } = new List<PaymentTemplate>();
    public ICollection<CreditApplication> CreditApplications { get; set; } = new List<CreditApplication>();
    public ICollection<UserDeposit> UserDeposits { get; set; } = new List<UserDeposit>();
}
