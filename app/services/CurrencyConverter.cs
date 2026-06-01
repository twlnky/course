using CourseBank.interfaces;
using CourseBank.model;

namespace CourseBank.services;

public class CurrencyConverter(IExchangeRateRepository exchangeRateRepository)
{
    public decimal Convert(decimal amount, CurrencyCode from, CurrencyCode to, out decimal rate)
    {
        if (from == to)
        {
            rate = 1m;
            return amount;
        }

        var direct = exchangeRateRepository.GetRate(from, to);
        if (direct != null)
        {
            rate = direct.Rate;
            return Math.Round(amount * rate, 2);
        }

        var inverse = exchangeRateRepository.GetRate(to, from);
        if (inverse != null && inverse.Rate != 0)
        {
            rate = 1m / inverse.Rate;
            return Math.Round(amount * rate, 2);
        }

        throw new AppException($"Курс обмена не найден: {from} → {to}");
    }
}
