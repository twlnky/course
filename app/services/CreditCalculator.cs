using CourseBank.model;

namespace CourseBank.services;

public static class CreditCalculator
{
    public static decimal CalculateMonthlyPayment(decimal amount, int termMonths, decimal annualRatePercent)
    {
        if (termMonths <= 0)
            throw new AppException("Срок вклада должен быть больше нуля.");

        if (annualRatePercent == 0)
            return Math.Round(amount / termMonths, 2);

        var monthlyRate = annualRatePercent / 100m / 12m;
        var factor = (decimal)Math.Pow((double)(1 + monthlyRate), termMonths);
        return Math.Round(amount * monthlyRate * factor / (factor - 1), 2);
    }

    public static decimal CalculateAccruedInterest(decimal amount, decimal ratePercentPerYear, int days) =>
        Math.Round(amount * ratePercentPerYear / 100m * days / 365m, 2);
}
