using CourseBank.dto.request;
using CourseBank.dto.response;
using CourseBank.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CourseBank.api;

public static class PaymentRecipientEndpoints
{
    public static RouteGroupBuilder MapPaymentRecipientEndpoints(this RouteGroupBuilder group)
    {
        var recipients = group.MapGroup("/payment-recipients").WithTags("Payment Recipients");

        recipients.MapGet("/", (string? name, IPaymentRecipientService service) =>
                Results.Ok(service.GetAll(name)))
            .WithSummary("Список получателей / поиск по имени")
            .Produces<IEnumerable<PaymentRecipientResponse>>(StatusCodes.Status200OK);

        recipients.MapGet("/{recipientId:int}", (int recipientId, IPaymentRecipientService service) =>
            {
                var recipient = service.GetById(recipientId);
                return recipient == null
                    ? Results.NotFound(new ErrorResponse { Message = "Получатель платежа не найден." })
                    : Results.Ok(recipient);
            })
            .WithSummary("Получатель по ID")
            .Produces<PaymentRecipientResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        recipients.MapPost("/", ([FromBody] CreatePaymentRecipientRequest request, IPaymentRecipientService service) =>
            {
                var recipient = service.Create(request);
                return Results.Created($"/api/payment-recipients/{recipient.Id}", recipient);
            })
            .WithSummary("Создать получателя")
            .Produces<PaymentRecipientResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        recipients.MapPut("/{recipientId:int}", (
                int recipientId,
                [FromBody] UpdatePaymentRecipientRequest request,
                IPaymentRecipientService service) =>
            {
                var recipient = service.Update(recipientId, request);
                return recipient == null
                    ? Results.NotFound(new ErrorResponse { Message = "Получатель платежа не найден." })
                    : Results.Ok(recipient);
            })
            .WithSummary("Обновить получателя")
            .Produces<PaymentRecipientResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        recipients.MapDelete("/{recipientId:int}", (int recipientId, IPaymentRecipientService service) =>
            {
                var deleted = service.Delete(recipientId);
                return deleted ? Results.NoContent() : Results.NotFound(new ErrorResponse { Message = "Получатель платежа не найден." });
            })
            .WithSummary("Удалить получателя")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        return group;
    }
}
