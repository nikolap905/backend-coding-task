using Claims.Auditing;
using Claims.Data;
using Claims.Models;
using Microsoft.EntityFrameworkCore;

namespace Claims.Services;

public class ClaimsService : IClaimsService
{
    private readonly ClaimsContext _context;
    private readonly Auditer _auditer;

    public ClaimsService(ClaimsContext context, Auditer auditer)
    {
        _context = context;
        _auditer = auditer;
    }

    public async Task<IEnumerable<Claim>> GetClaimsAsync()
    {
        return await _context.Claims.ToListAsync();
    }

    public async Task<Claim?> GetClaimAsync(string id)
    {
        return await _context.Claims.SingleOrDefaultAsync(claim => claim.Id == id);
    }

    public async Task<Claim> CreateClaimAsync(Claim claim)
    {
        claim.Id = Guid.NewGuid().ToString();
        _context.Claims.Add(claim);
        await _context.SaveChangesAsync();
        _auditer.AuditClaim(claim.Id, "POST");
        return claim;
    }

    public async Task<bool> DeleteClaimAsync(string id)
    {
        _auditer.AuditClaim(id, "DELETE");

        var claim = await GetClaimAsync(id);
        if (claim is null)
        {
            return false;
        }

        _context.Claims.Remove(claim);
        await _context.SaveChangesAsync();
        return true;
    }
}
