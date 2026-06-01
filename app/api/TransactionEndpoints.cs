using CourseBank.dto.request;
using CourseBank.dto.response;
using CourseBank.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CourseBank.api;

public static class TransactionEndpoints
{
    public static RouteGroupBuilder MapTransactionEndpoints(this RouteGroupBuilder group)
    {
        var transactions = group.MapGroup("/transactions").WithTags("Transactions");

        transactions.MapGet("/account/{accountId:int}", (int accountId, ITransactionService service) =>
                Results.Ok(service.GetByAccountId(accountId)))
            .WithSummary("История транзакций по счёту")
            .Produces<IEnumerable<TransactionResponse>>(StatusCodes.Status200OK);

        transactions.MapGet("/{transactionId:int}", (int transactionId, ITransactionService service) =>
            {
                var transaction = service.GetById(transactionId);
                return transaction == null
                    ? Results.NotFound(new ErrorResponse { Message = "Транзакция не найдена." })
                    : Results.Ok(transaction);
            })
            .WithSummary("Транзакция по ID")
            .Produces<TransactionResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        transactions.MapPost("/", ([FromBody] CreateTransactionRequest request, ITransactionService service) =>
            {
                var transaction = service.Create(request);
                return Results.Created($"/api/transactions/{transaction.Id}", transaction);
            })
            .WithSummary("Создать транзакцию (Pending, без проводки)")
            .Produces<TransactionResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        transactions.MapPut("/{transactionId:int}/status", (
                int transactionId,
                [FromBody] UpdateTransactionStatusRequest request,
                ITransactionService service) =>
            {
                var transaction = service.UpdateStatus(transactionId, request);
                return transaction == null
                    ? Results.NotFound(new ErrorResponse { Message = "Транзакция не найдена." })
                    : Results.Ok(transaction);
            })
            .WithSummary("Смена статуса + проводка / откат")
            .Produces<TransactionResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        transactions.MapPost("/transfer", ([FromBody] TransferRequest request, ITransactionService service) =>
                Results.Ok(service.Transfer(request)))
            .WithSummary("Перевод между счетами")
            .Produces<OperationResultResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        transactions.MapPost("/credit-repayment", ([FromBody] CreditRepaymentRequest request, ITransactionService service) =>
                Results.Ok(service.RepayCredit(request)))
            .WithSummary("Погашение кредита")
            .Produces<OperationResultResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        return group;
    }
}
