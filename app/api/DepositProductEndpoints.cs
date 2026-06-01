using CourseBank.dto.request;
using CourseBank.dto.response;
using CourseBank.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CourseBank.api;

public static class DepositProductEndpoints
{
    public static RouteGroupBuilder MapDepositProductEndpoints(this RouteGroupBuilder group)
    {
        var products = group.MapGroup("/deposit-products").WithTags("Deposit Products");

        products.MapGet("/", (IDepositService service) => Results.Ok(service.GetAllProducts()))
            .WithSummary("Список продуктов вкладов")
            .Produces<IEnumerable<DepositProductResponse>>(StatusCodes.Status200OK);

        products.MapGet("/{productId:int}", (int productId, IDepositService service) =>
            {
                var product = service.GetProductById(productId);
                return product == null
                    ? Results.NotFound(new ErrorResponse { Message = "Продукт вклада не найден." })
                    : Results.Ok(product);
            })
            .WithSummary("Продукт по ID")
            .Produces<DepositProductResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        products.MapPost("/", ([FromBody] CreateDepositProductRequest request, IDepositService service) =>
            {
                var product = service.CreateProduct(request);
                return Results.Created($"/api/deposit-products/{product.Id}", product);
            })
            .WithSummary("Создать продукт вклада")
            .Produces<DepositProductResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        return group;
    }
}
