using CourseBank.dto.request;
using CourseBank.dto.response;
using CourseBank.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CourseBank.api;

public static class AccountEndpoints
{
    public static RouteGroupBuilder MapAccountEndpoints(this RouteGroupBuilder group)
    {
        var accounts = group.MapGroup("/accounts").WithTags("Accounts");

        accounts.MapGet("/user/{userId:int}", (int userId, IAccountService service) =>
                Results.Ok(service.GetByUserId(userId)))
            .WithSummary("Счета пользователя")
            .Produces<IEnumerable<AccountResponse>>(StatusCodes.Status200OK);

        accounts.MapGet("/{accountId:int}", (int accountId, IAccountService service) =>
            {
                var account = service.GetById(accountId);
                return account == null
                    ? Results.NotFound(new ErrorResponse { Message = "Счёт не найден." })
                    : Results.Ok(account);
            })
            .WithSummary("Счёт по ID")
            .Produces<AccountResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        accounts.MapGet("/{accountId:int}/balance", (int accountId, IAccountService service) =>
            {
                var balance = service.GetBalance(accountId);
                return balance == null
                    ? Results.NotFound(new ErrorResponse { Message = "Счёт не найден." })
                    : Results.Ok(balance);
            })
            .WithSummary("Баланс счёта")
            .Produces<AccountBalanceResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        accounts.MapPost("/", ([FromBody] OpenAccountRequest request, IAccountService service) =>
            {
                var account = service.OpenAccount(request);
                return Results.Created($"/api/accounts/{account.Id}", account);
            })
            .WithSummary("Открыть счёт")
            .Produces<AccountResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ErrorResponse>(StatusCodes.Status409Conflict);

        accounts.MapPut("/{accountId:int}", (int accountId, [FromBody] UpdateAccountRequest request, IAccountService service) =>
            {
                var account = service.Update(accountId, request);
                return account == null
                    ? Results.NotFound(new ErrorResponse { Message = "Счёт не найден." })
                    : Results.Ok(account);
            })
            .WithSummary("Обновить счёт")
            .Produces<AccountResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        accounts.MapPut("/{accountId:int}/block", (int accountId, [FromBody] BlockAccountRequest request, IAccountService service) =>
            {
                var account = service.SetBlocked(accountId, request.IsBlocked);
                return account == null
                    ? Results.NotFound(new ErrorResponse { Message = "Счёт не найден." })
                    : Results.Ok(account);
            })
            .WithSummary("Заблокировать / разблокировать счёт")
            .Produces<AccountResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        accounts.MapDelete("/{accountId:int}", (int accountId, IAccountService service) =>
            {
                var deleted = service.SoftDelete(accountId);
                return deleted ? Results.NoContent() : Results.NotFound(new ErrorResponse { Message = "Счёт не найден." });
            })
            .WithSummary("Soft delete счёта")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        return group;
    }
}
