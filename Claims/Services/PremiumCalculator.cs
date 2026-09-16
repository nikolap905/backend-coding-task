using Claims.Models;

namespace Claims.Services;

public class PremiumCalculator : IPremiumCalculator
{
    private const decimal BaseDayRate = 1250m;
    private const int Tier1Days = 30;
    private const int Tier2Days = 150;

    public decimal Calculate(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        var totalDays = (endDate.Date - startDate.Date).Days;
        if (totalDays <= 0)
        {
            return 0m;
        }

        var dailyRate = GetDailyRate(coverType);
        var isYacht = coverType == CoverType.Yacht;

        var tier1Days = Math.Min(totalDays, Tier1Days);
        var tier2Days = Math.Min(Math.Max(totalDays - Tier1Days, 0), Tier2Days);
        var tier3Days = Math.Max(totalDays - Tier1Days - Tier2Days, 0);

        var tier2Rate = dailyRate * (isYacht ? 0.95m : 0.98m);
        var tier3Rate = dailyRate * (isYacht ? 0.92m : 0.97m);

        return (tier1Days * dailyRate) + (tier2Days * tier2Rate) + (tier3Days * tier3Rate);
    }

    private static decimal GetDailyRate(CoverType coverType) => coverType switch
    {
        CoverType.Yacht => BaseDayRate * 1.1m,
        CoverType.PassengerShip => BaseDayRate * 1.2m,
        CoverType.Tanker => BaseDayRate * 1.5m,
        _ => BaseDayRate * 1.3m
    };
}
