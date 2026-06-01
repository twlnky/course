namespace CourseBank.dto.request;

public class UpdateUserRequest
{
    public string? FullName { get; set; }
    public string? Password { get; set; }
    public bool? IsActive { get; set; }
}
