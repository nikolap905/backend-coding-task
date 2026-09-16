using Claims.Models;

namespace Claims.Services;

public interface IPremiumCalculator
{
    decimal Calculate(DateTime startDate, DateTime endDate, CoverType coverType);
}
