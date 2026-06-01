using CourseBank.model;

namespace CourseBank.dto.response;

public class UserResponse
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime RegistrationDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
}
