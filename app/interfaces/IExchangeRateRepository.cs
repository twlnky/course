using CourseBank.model;

namespace CourseBank.interfaces;

public interface IExchangeRateRepository
{
    IEnumerable<ExchangeRate> GetAll();
    ExchangeRate? GetRate(CurrencyCode from, CurrencyCode to);
    bool IsEmpty();
    void AddRange(IEnumerable<ExchangeRate> rates);
}
