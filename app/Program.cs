using CourseBank.api;
using CourseBank.database;
using CourseBank.database.repositories;
using CourseBank.dto;
using CourseBank.interfaces;
using CourseBank.model;
using CourseBank.services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("Postgres");
builder.Services.AddDbContext<CourseDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ICreditApplicationRepository, CreditApplicationRepository>();
builder.Services.AddScoped<IPaymentRecipientRepository, PaymentRecipientRepository>();
builder.Services.AddScoped<IPaymentTemplateRepository, PaymentTemplateRepository>();
builder.Services.AddScoped<IDepositProductRepository, DepositProductRepository>();
builder.Services.AddScoped<IUserDepositRepository, UserDepositRepository>();
builder.Services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();

builder.Services.AddScoped<IMapper, Mapper>();
builder.Services.AddScoped<CurrencyConverter>();
builder.Services.AddScoped<TransactionSettlement>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPaymentRecipientService, PaymentRecipientService>();
builder.Services.AddScoped<IPaymentTemplateService, PaymentTemplateService>();
builder.Services.AddScoped<ICreditApplicationService, CreditApplicationService>();
builder.Services.AddScoped<IDepositService, DepositService>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseRequestLogging();

if (app.Environment.IsDevelopment())
{
    var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<CourseDbContext>();
    try
    {
        await db.Database.EnsureCreatedAsync();
        SeedExchangeRates(scope.ServiceProvider);
    }
    catch (Exception ex) when (IsDatabaseUnavailable(ex))
    {
        throw new InvalidOperationException(
            "бд недоступна" +
            ex);
    }

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Course Bank API v1");
        options.RoutePrefix = "swagger";
    });

    app.MapOpenApi();
}

app.MapGet("/", () => Results.Ok(new
{
    endpoints = new[]
    {
        "/api/users",
        "/api/accounts",
        "/api/transactions",
        "/api/payment-recipients",
        "/api/payments",
        "/api/payment-templates",
        "/api/credit-applications",
        "/api/deposit-products",
        "/api/user-deposits",
        "/swagger"
    }
})).WithTags("System");

var api = app.MapGroup("/api");
api.MapUserEndpoints();
api.MapAccountEndpoints();
api.MapTransactionEndpoints();
api.MapPaymentRecipientEndpoints();
api.MapPaymentEndpoints();
api.MapPaymentTemplateEndpoints();
api.MapCreditApplicationEndpoints();
api.MapDepositProductEndpoints();
api.MapUserDepositEndpoints();

app.Run();

static void SeedExchangeRates(IServiceProvider services)
{
    var repository = services.GetRequiredService<IExchangeRateRepository>();
    if (!repository.IsEmpty()) return;

    var now = DateTime.UtcNow;
    repository.AddRange(
    [
        new ExchangeRate { FromCurrency = CurrencyCode.RUB, ToCurrency = CurrencyCode.USD, Rate = 0.010800m, UpdatedAt = now },
        new ExchangeRate { FromCurrency = CurrencyCode.RUB, ToCurrency = CurrencyCode.EUR, Rate = 0.010000m, UpdatedAt = now },
        new ExchangeRate { FromCurrency = CurrencyCode.USD, ToCurrency = CurrencyCode.RUB, Rate = 92.592593m, UpdatedAt = now },
        new ExchangeRate { FromCurrency = CurrencyCode.USD, ToCurrency = CurrencyCode.EUR, Rate = 0.925926m, UpdatedAt = now },
        new ExchangeRate { FromCurrency = CurrencyCode.EUR, ToCurrency = CurrencyCode.RUB, Rate = 100.000000m, UpdatedAt = now },
        new ExchangeRate { FromCurrency = CurrencyCode.EUR, ToCurrency = CurrencyCode.USD, Rate = 1.080000m, UpdatedAt = now }
    ]);

    services.GetRequiredService<IUnitOfWork>().SaveChanges();
}

static bool IsDatabaseUnavailable(Exception ex)
{
    for (var current = ex; current != null; current = current.InnerException)
    {
        if (current is System.Net.Sockets.SocketException { SocketErrorCode: System.Net.Sockets.SocketError.ConnectionRefused })
            return true;
        if (current is Npgsql.NpgsqlException)
            return true;
    }

    return false;
}
