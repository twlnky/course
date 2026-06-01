using CourseBank.dto.request;
using CourseBank.dto.response;
using CourseBank.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CourseBank.api;

public static class PaymentEndpoints
{
    public static RouteGroupBuilder MapPaymentEndpoints(this RouteGroupBuilder group)
    {
        var payments = group.MapGroup("/payments").WithTags("Payments");

        payments.MapPost("/execute", ([FromBody] ExecutePaymentRequest request, IPaymentService service) =>
                Results.Ok(service.ExecutePayment(request)))
            .WithSummary("Исполнить платёж получателю")
            .Produces<OperationResultResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        payments.MapPost("/execute-by-template", ([FromBody] ExecuteByTemplateRequest request, IPaymentService service) =>
                Results.Ok(service.ExecuteByTemplate(request)))
            .WithSummary("Платёж по шаблону")
            .Produces<OperationResultResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        return group;
    }
}
