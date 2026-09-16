using System.ComponentModel.DataAnnotations;
using Claims.Models;
using Xunit;

namespace Claims.Tests.Models;

public class ClaimValidationTests
{
    private static IList<ValidationResult> Validate(Claim claim)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(claim, new ValidationContext(claim), results, validateAllProperties: true);
        return results;
    }

    [Fact]
    public void DamageCostOverLimit_IsInvalid()
    {
        var claim = new Claim { CoverId = "c1", Name = "Test", Created = DateTime.UtcNow, DamageCost = 100_001 };

        var results = Validate(claim);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(Claim.DamageCost)));
    }

    [Fact]
    public void DamageCostUnderLimit_IsValid()
    {
        var claim = new Claim { CoverId = "c1", Name = "Test", Created = DateTime.UtcNow, DamageCost = 99_999 };

        var results = Validate(claim);

        Assert.DoesNotContain(results, r => r.MemberNames.Contains(nameof(Claim.DamageCost)));
    }
}
