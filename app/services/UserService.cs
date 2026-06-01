using CourseBank.dto.request;
using CourseBank.dto.response;
using CourseBank.interfaces;
using CourseBank.model;
using System.Text;

namespace CourseBank.services;

public class UserService(
    IUserRepository userRepository,
    IAccountRepository accountRepository,
    IPaymentTemplateRepository paymentTemplateRepository,
    IUnitOfWork unitOfWork) : IUserService
{
    public IEnumerable<UserResponse> GetAll() =>
        userRepository.GetAll().Select(EntityMapper.ToUserResponse);

    public UserResponse? GetById(int userId)
    {
        var user = userRepository.GetById(userId);
        return user == null ? null : EntityMapper.ToUserResponse(user);
    }

    public UserResponse Register(RegisterUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Password))
            throw new AppException("Логин и пароль обязательны");

        if (userRepository.LoginExists(request.Login))
            throw new AppException("Логин уже занят", StatusCodes.Status409Conflict);

        var user = new User
        {
            Login = request.Login,
            Password = request.Password,
            FullName = request.FullName,
            Role = request.Role,
            IsActive = true,
            RegistrationDate = DateTime.UtcNow
        };

        userRepository.Add(user);
        unitOfWork.SaveChanges();
        return EntityMapper.ToUserResponse(user);
    }

    public UserResponse? Update(int userId, UpdateUserRequest request)
    {
        var user = userRepository.GetByIdForUpdate(userId);
        if (user == null) return null;

        if (!string.IsNullOrWhiteSpace(request.FullName))
            user.FullName = request.FullName;
        if (!string.IsNullOrWhiteSpace(request.Password))
            user.Password = request.Password;
        if (request.IsActive.HasValue)
            user.IsActive = request.IsActive.Value;

        userRepository.Update(user);
        unitOfWork.SaveChanges();
        return EntityMapper.ToUserResponse(user);
    }

    public bool SoftDelete(int userId)
    {
        var user = userRepository.GetByIdForUpdate(userId);
        if (user == null) return false;

        var accounts = userRepository.GetAccountsForUserSoftDelete(userId).ToList();
        foreach (var account in accounts)
        {
            if (account.Balance != 0)
                throw new AppException("Нельзя удалить пользователя: на счёте ненулевой баланс");
            if (account.AccountType == AccountType.Credit && (account.DebtBalance ?? 0) > 0)
                throw new AppException("Нельзя удалить пользователя: остался кредитный долг");
        }

        foreach (var account in accounts)
        {
            account.IsDeleted = true;
            account.DeletedAt = DateTime.UtcNow;
            accountRepository.Update(account);
        }

        paymentTemplateRepository.DeactivateByUserId(userId);
        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
        user.IsActive = false;
        userRepository.Update(user);
        unitOfWork.SaveChanges();
        return true;
    }

    public LoginResponse Login(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Password))
            throw new AppException("Логин и пароль обязательны", StatusCodes.Status401Unauthorized);

        var user = userRepository.GetByLogin(request.Login);
        if (user == null || user.Password != request.Password)
            throw new AppException("Неверный логин или пароль", StatusCodes.Status401Unauthorized);

        if (!user.IsActive)
            throw new AppException("Учётная запись деактивирована", StatusCodes.Status403Forbidden);

        user.LastLoginDate = DateTime.UtcNow;
        userRepository.Update(user);
        unitOfWork.SaveChanges();

        var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user.Login}:{user.Password}"));
        return new LoginResponse
        {
            UserId = user.Id,
            Login = user.Login,
            FullName = user.FullName,
            Token = token
        };
    }
}
