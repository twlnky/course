using CourseBank.dto.request;
using CourseBank.dto.response;
using CourseBank.model;

namespace CourseBank.interfaces;

public interface IUserService
{
    IEnumerable<UserResponse> GetAll();
    UserResponse? GetById(int userId);
    UserResponse Register(RegisterUserRequest request);
    UserResponse? Update(int userId, UpdateUserRequest request);
    bool SoftDelete(int userId);
    LoginResponse Login(LoginRequest request);
}
