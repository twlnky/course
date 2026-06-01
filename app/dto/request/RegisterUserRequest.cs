using CourseBank.model;

namespace CourseBank.dto.request;

public record RegisterUserRequest
{
    public string Login { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public UserRole Role { get; init; } = UserRole.Client;
}
