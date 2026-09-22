using Claims.Domain;

namespace Claims.Application;

public interface IPremiumCalculator
{
    decimal Calculate(DateTime startDate, DateTime endDate, CoverType coverType);
}
