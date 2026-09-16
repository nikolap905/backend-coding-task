using System.ComponentModel.DataAnnotations;
using Claims.Models;
using Xunit;

namespace Claims.Tests.Models;

public class CoverValidationTests
{
    private static IList<ValidationResult> Validate(Cover cover)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(cover, new ValidationContext(cover), results, validateAllProperties: true);
        return results;
    }

    [Fact]
    public void PastStartDate_IsInvalid()
    {
        var cover = new Cover { StartDate = DateTime.UtcNow.AddDays(-1), EndDate = DateTime.UtcNow.AddDays(30) };

        var results = Validate(cover);

        Assert.Contains(results, r => r.ErrorMessage!.Contains("cannot be in the past"));
    }

    [Fact]
    public void EndDateBeforeStartDate_IsInvalid()
    {
        var cover = new Cover { StartDate = DateTime.UtcNow.AddDays(10), EndDate = DateTime.UtcNow.AddDays(5) };

        var results = Validate(cover);

        Assert.Contains(results, r => r.ErrorMessage!.Contains("must be after"));
    }

    [Fact]
    public void PeriodExceeds1Year_IsInvalid()
    {
        var cover = new Cover { StartDate = DateTime.UtcNow.AddDays(1), EndDate = DateTime.UtcNow.AddDays(400) };

        var results = Validate(cover);

        Assert.Contains(results, r => r.ErrorMessage!.Contains("cannot exceed 1 year"));
    }

    [Fact]
    public void ValidPeriod_HasNoErrors()
    {
        var cover = new Cover { StartDate = DateTime.UtcNow.Date, EndDate = DateTime.UtcNow.Date.AddDays(30) };

        var results = Validate(cover);

        Assert.Empty(results);
    }
}
