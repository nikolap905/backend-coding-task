using Claims.Data;
using Claims.Models;
using Claims.Services;
using Claims.Tests.Fakes;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Claims.Tests.Services;

public class ClaimsServiceTests
{
    private static ClaimsContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ClaimsContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ClaimsContext(options);
    }

    [Fact]
    public async Task CreateClaimAsync_ValidClaim_SavesAndAudits()
    {
        using var context = CreateContext();
        var auditer = new FakeAuditer();
        var cover = new Cover { Id = "cover-1", StartDate = DateTime.UtcNow.AddDays(-5), EndDate = DateTime.UtcNow.AddDays(30) };
        context.Covers.Add(cover);
        await context.SaveChangesAsync();

        var service = new ClaimsService(context, auditer);
        var claim = new Claim { CoverId = cover.Id!, Created = DateTime.UtcNow, Name = "Test", DamageCost = 500 };

        var (created, error) = await service.CreateClaimAsync(claim);

        Assert.Null(error);
        Assert.NotNull(created);
        Assert.Single(context.Claims);
        Assert.Single(auditer.ClaimAudits);
        Assert.Equal("POST", auditer.ClaimAudits[0].HttpRequestType);
    }

    [Fact]
    public async Task CreateClaimAsync_CreatedBeforeCoverStart_ReturnsError()
    {
        using var context = CreateContext();
        var auditer = new FakeAuditer();
        var cover = new Cover { Id = "cover-1", StartDate = DateTime.UtcNow.AddDays(5), EndDate = DateTime.UtcNow.AddDays(30) };
        context.Covers.Add(cover);
        await context.SaveChangesAsync();

        var service = new ClaimsService(context, auditer);
        var claim = new Claim { CoverId = cover.Id!, Created = DateTime.UtcNow, Name = "Test", DamageCost = 500 };

        var (created, error) = await service.CreateClaimAsync(claim);

        Assert.Null(created);
        Assert.NotNull(error);
        Assert.Empty(context.Claims);
        Assert.Empty(auditer.ClaimAudits);
    }

    [Fact]
    public async Task CreateClaimAsync_CreatedAfterCoverEnd_ReturnsError()
    {
        using var context = CreateContext();
        var auditer = new FakeAuditer();
        var cover = new Cover { Id = "cover-1", StartDate = DateTime.UtcNow.AddDays(-30), EndDate = DateTime.UtcNow.AddDays(-5) };
        context.Covers.Add(cover);
        await context.SaveChangesAsync();

        var service = new ClaimsService(context, auditer);
        var claim = new Claim { CoverId = cover.Id!, Created = DateTime.UtcNow, Name = "Test", DamageCost = 500 };

        var (created, error) = await service.CreateClaimAsync(claim);

        Assert.Null(created);
        Assert.NotNull(error);
    }

    [Fact]
    public async Task CreateClaimAsync_CoverDoesNotExist_ReturnsError()
    {
        using var context = CreateContext();
        var auditer = new FakeAuditer();
        var service = new ClaimsService(context, auditer);
        var claim = new Claim { CoverId = "missing-cover", Created = DateTime.UtcNow, Name = "Test", DamageCost = 500 };

        var (created, error) = await service.CreateClaimAsync(claim);

        Assert.Null(created);
        Assert.NotNull(error);
        Assert.Contains("not found", error);
    }
}
