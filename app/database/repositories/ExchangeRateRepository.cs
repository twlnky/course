using CourseBank.interfaces;
using CourseBank.model;
using Microsoft.EntityFrameworkCore;

namespace CourseBank.database.repositories;

public class ExchangeRateRepository(CourseDbContext db) : IExchangeRateRepository
{
    public IEnumerable<ExchangeRate> GetAll() =>
        db.ExchangeRates.AsNoTracking().ToList();

    public ExchangeRate? GetRate(CurrencyCode from, CurrencyCode to) =>
        db.ExchangeRates.AsNoTracking().FirstOrDefault(r => r.FromCurrency == from && r.ToCurrency == to);

    public bool IsEmpty() => !db.ExchangeRates.Any();

    public void AddRange(IEnumerable<ExchangeRate> rates) => db.ExchangeRates.AddRange(rates);
}
