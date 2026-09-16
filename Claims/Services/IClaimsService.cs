using Claims.Models;

namespace Claims.Services;

public interface IClaimsService
{
    Task<IEnumerable<Claim>> GetClaimsAsync();

    Task<Claim?> GetClaimAsync(string id);

    Task<(Claim? Claim, string? Error)> CreateClaimAsync(Claim claim);

    Task<bool> DeleteClaimAsync(string id);
}
