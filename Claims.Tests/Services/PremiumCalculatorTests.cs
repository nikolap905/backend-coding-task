using Claims.Models;
using Claims.Services;
using Xunit;

namespace Claims.Tests.Services;

public class PremiumCalculatorTests
{
    private readonly PremiumCalculator _calculator = new();

    [Theory]
    [InlineData(CoverType.Yacht, 41250)]
    [InlineData(CoverType.PassengerShip, 45000)]
    [InlineData(CoverType.Tanker, 56250)]
    [InlineData(CoverType.ContainerShip, 48750)]
    [InlineData(CoverType.BulkCarrier, 48750)]
    public void Calculate_30DayPeriod_UsesFullDailyRateForVesselType(CoverType coverType, decimal expectedPremium)
    {
        var start = new DateTime(2026, 1, 1);
        var end = start.AddDays(30);

        var premium = _calculator.Calculate(start, end, coverType);

        Assert.Equal(expectedPremium, premium);
    }

    [Fact]
    public void Calculate_31Days_OtherType_AddsOneDayAtTier2Rate()
    {
        var start = new DateTime(2026, 1, 1);
        var end = start.AddDays(31);

        var premium = _calculator.Calculate(start, end, CoverType.ContainerShip);

        // 30 days @ 1625 (full) + 1 day @ 1625 * 0.98 (2% off)
        Assert.Equal(30 * 1625m + 1 * (1625m * 0.98m), premium);
    }

    [Fact]
    public void Calculate_180Days_OtherType_AllTier2DaysAtDiscountedRate()
    {
        var start = new DateTime(2026, 1, 1);
        var end = start.AddDays(180);

        var premium = _calculator.Calculate(start, end, CoverType.ContainerShip);

        // 30 days @ full rate + 150 days @ 2% off
        Assert.Equal(30 * 1625m + 150 * (1625m * 0.98m), premium);
    }

    [Fact]
    public void Calculate_181Days_OtherType_AddsOneDayAtTier3Rate()
    {
        var start = new DateTime(2026, 1, 1);
        var end = start.AddDays(181);

        var premium = _calculator.Calculate(start, end, CoverType.ContainerShip);

        // 30 days @ full rate + 150 days @ 2% off + 1 day @ 3% off
        Assert.Equal(30 * 1625m + 150 * (1625m * 0.98m) + 1 * (1625m * 0.97m), premium);
    }

    [Fact]
    public void Calculate_181Days_Yacht_UsesYachtTier3Discount()
    {
        var start = new DateTime(2026, 1, 1);
        var end = start.AddDays(181);

        var premium = _calculator.Calculate(start, end, CoverType.Yacht);

        // 30 days @ full rate + 150 days @ 5% off + 1 day @ 8% off (additive: 5% + 3%)
        Assert.Equal(30 * 1375m + 150 * (1375m * 0.95m) + 1 * (1375m * 0.92m), premium);
    }

    [Fact]
    public void Calculate_EndDateNotAfterStartDate_ReturnsZero()
    {
        var date = new DateTime(2026, 1, 1);

        var premium = _calculator.Calculate(date, date, CoverType.Yacht);

        Assert.Equal(0m, premium);
    }
}
