using CourseBank.dto.request;
using CourseBank.dto.response;
using CourseBank.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CourseBank.api;

public static class UserEndpoints
{
    public static RouteGroupBuilder MapUserEndpoints(this RouteGroupBuilder group)
    {
        var users = group.MapGroup("/users").WithTags("Users");

        users.MapGet("/", (IUserService service) => Results.Ok(service.GetAll()))
            .WithSummary("Список пользователей")
            .Produces<IEnumerable<UserResponse>>(StatusCodes.Status200OK);

        users.MapGet("/{userId:int}", (int userId, IUserService service) =>
            {
                var user = service.GetById(userId);
                return user == null
                    ? Results.NotFound(new ErrorResponse { Message = "Пользователь не найден." })
                    : Results.Ok(user);
            })
            .WithSummary("Пользователь по ID")
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        users.MapPost("/", ([FromBody] RegisterUserRequest request, IUserService service) =>
            {
                var user = service.Register(request);
                return Results.Created($"/api/users/{user.Id}", user);
            })
            .WithSummary("Регистрация пользователя")
            .Produces<UserResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status409Conflict);

        users.MapPut("/{userId:int}", (int userId, [FromBody] UpdateUserRequest request, IUserService service) =>
            {
                var user = service.Update(userId, request);
                return user == null
                    ? Results.NotFound(new ErrorResponse { Message = "Пользователь не найден." })
                    : Results.Ok(user);
            })
            .WithSummary("Обновление профиля")
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        users.MapDelete("/{userId:int}", (int userId, IUserService service) =>
            {
                var deleted = service.SoftDelete(userId);
                return deleted ? Results.NoContent() : Results.NotFound(new ErrorResponse { Message = "Пользователь не найден." });
            })
            .WithSummary("Soft delete пользователя")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        users.MapPost("/login", ([FromBody] LoginRequest request, IUserService service) =>
                Results.Ok(service.Login(request)))
            .WithSummary("Логин")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ErrorResponse>(StatusCodes.Status403Forbidden);

        return group;
    }
}
