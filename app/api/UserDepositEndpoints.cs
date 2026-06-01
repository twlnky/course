using CourseBank.dto.request;
using CourseBank.dto.response;
using CourseBank.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CourseBank.api;

public static class UserDepositEndpoints
{
    public static RouteGroupBuilder MapUserDepositEndpoints(this RouteGroupBuilder group)
    {
        var deposits = group.MapGroup("/user-deposits").WithTags("User Deposits");

        deposits.MapGet("/user/{userId:int}", (int userId, IDepositService service) =>
                Results.Ok(service.GetUserDeposits(userId)))
            .WithSummary("Вклады пользователя")
            .Produces<IEnumerable<UserDepositResponse>>(StatusCodes.Status200OK);

        deposits.MapGet("/{userDepositId:int}", (int userDepositId, IDepositService service) =>
            {
                var deposit = service.GetUserDepositById(userDepositId);
                return deposit == null
                    ? Results.NotFound(new ErrorResponse { Message = "Вклад не найден." })
                    : Results.Ok(deposit);
            })
            .WithSummary("Вклад по ID")
            .Produces<UserDepositResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        deposits.MapPost("/open", ([FromBody] OpenUserDepositRequest request, IDepositService service) =>
            {
                var deposit = service.OpenDeposit(request);
                return Results.Created($"/api/user-deposits/{deposit.Id}", deposit);
            })
            .WithSummary("Открыть вклад")
            .Produces<UserDepositResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ErrorResponse>(StatusCodes.Status409Conflict);

        deposits.MapPost("/early-withdraw", ([FromBody] EarlyWithdrawRequest request, IDepositService service) =>
                Results.Ok(service.EarlyWithdraw(request)))
            .WithSummary("Досрочное закрытие вклада")
            .Produces<DepositClosureResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        return group;
    }
}
