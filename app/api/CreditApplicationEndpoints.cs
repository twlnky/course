using CourseBank.dto.request;
using CourseBank.dto.response;
using CourseBank.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CourseBank.api;

public static class CreditApplicationEndpoints
{
    public static RouteGroupBuilder MapCreditApplicationEndpoints(this RouteGroupBuilder group)
    {
        var applications = group.MapGroup("/credit-applications").WithTags("Credit Applications");

        applications.MapGet("/user/{userId:int}", (int userId, ICreditApplicationService service) =>
                Results.Ok(service.GetByUserId(userId)))
            .WithSummary("Заявки пользователя")
            .Produces<IEnumerable<CreditApplicationResponse>>(StatusCodes.Status200OK);

        applications.MapGet("/{applicationId:int}", (int applicationId, ICreditApplicationService service) =>
            {
                var application = service.GetById(applicationId);
                return application == null
                    ? Results.NotFound(new ErrorResponse { Message = "Кредитная заявка не найдена." })
                    : Results.Ok(application);
            })
            .WithSummary("Заявка по ID")
            .Produces<CreditApplicationResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        applications.MapPost("/", ([FromBody] SubmitCreditApplicationRequest request, ICreditApplicationService service) =>
            {
                var application = service.Submit(request);
                return Results.Created($"/api/credit-applications/{application.Id}", application);
            })
            .WithSummary("Подать кредитную заявку")
            .Produces<CreditApplicationResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status409Conflict);

        applications.MapPut("/{applicationId:int}/review", (
                int applicationId,
                [FromBody] ReviewCreditApplicationRequest request,
                ICreditApplicationService service) =>
            {
                var application = service.Review(applicationId, request);
                return application == null
                    ? Results.NotFound(new ErrorResponse { Message = "Кредитная заявка не найдена." })
                    : Results.Ok(application);
            })
            .WithSummary("Одобрить / отклонить заявку (оператор)")
            .Produces<CreditApplicationResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        applications.MapPost("/issue", ([FromBody] IssueCreditRequest request, ICreditApplicationService service) =>
                Results.Ok(service.IssueCredit(request)))
            .WithSummary("Выдать кредит (создать Credit-счёт)")
            .Produces<AccountResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ErrorResponse>(StatusCodes.Status409Conflict);

        return group;
    }
}
