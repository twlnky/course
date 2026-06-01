namespace CourseBank.dto.request;

public record LoginRequest
{
    public string Login { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
