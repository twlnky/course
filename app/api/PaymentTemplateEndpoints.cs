using CourseBank.dto.request;
using CourseBank.dto.response;
using CourseBank.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CourseBank.api;

public static class PaymentTemplateEndpoints
{
    public static RouteGroupBuilder MapPaymentTemplateEndpoints(this RouteGroupBuilder group)
    {
        var templates = group.MapGroup("/payment-templates").WithTags("Payment Templates");

        templates.MapGet("/user/{userId:int}", (int userId, IPaymentTemplateService service) =>
                Results.Ok(service.GetByUserId(userId)))
            .WithSummary("Шаблоны пользователя")
            .Produces<IEnumerable<PaymentTemplateResponse>>(StatusCodes.Status200OK);

        templates.MapGet("/{templateId:int}", (int templateId, IPaymentTemplateService service) =>
            {
                var template = service.GetById(templateId);
                return template == null
                    ? Results.NotFound(new ErrorResponse { Message = "Шаблон платежа не найден." })
                    : Results.Ok(template);
            })
            .WithSummary("Шаблон по ID")
            .Produces<PaymentTemplateResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        templates.MapPost("/", ([FromBody] CreatePaymentTemplateRequest request, IPaymentTemplateService service) =>
            {
                var template = service.Create(request);
                return Results.Created($"/api/payment-templates/{template.Id}", template);
            })
            .WithSummary("Создать шаблон")
            .Produces<PaymentTemplateResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        templates.MapPut("/{templateId:int}", (
                int templateId,
                [FromBody] UpdatePaymentTemplateRequest request,
                IPaymentTemplateService service) =>
            {
                var template = service.Update(templateId, request);
                return template == null
                    ? Results.NotFound(new ErrorResponse { Message = "Шаблон платежа не найден." })
                    : Results.Ok(template);
            })
            .WithSummary("Обновить шаблон")
            .Produces<PaymentTemplateResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        templates.MapDelete("/{templateId:int}", (int templateId, IPaymentTemplateService service) =>
            {
                var deleted = service.Delete(templateId);
                return deleted ? Results.NoContent() : Results.NotFound(new ErrorResponse { Message = "Шаблон платежа не найден." });
            })
            .WithSummary("Удалить шаблон")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        return group;
    }
}
