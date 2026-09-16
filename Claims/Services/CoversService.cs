using Claims.Auditing;
using Claims.Data;
using Claims.Models;
using Microsoft.EntityFrameworkCore;

namespace Claims.Services;

public class CoversService : ICoversService
{
    private readonly ClaimsContext _context;
    private readonly Auditer _auditer;
    private readonly IPremiumCalculator _premiumCalculator;

    public CoversService(ClaimsContext context, Auditer auditer, IPremiumCalculator premiumCalculator)
    {
        _context = context;
        _auditer = auditer;
        _premiumCalculator = premiumCalculator;
    }

    public async Task<IEnumerable<Cover>> GetCoversAsync()
    {
        return await _context.Covers.ToListAsync();
    }

    public async Task<Cover?> GetCoverAsync(string id)
    {
        return await _context.Covers.SingleOrDefaultAsync(cover => cover.Id == id);
    }

    public async Task<Cover> CreateCoverAsync(Cover cover)
    {
        cover.Id = Guid.NewGuid().ToString();
        cover.Premium = _premiumCalculator.Calculate(cover.StartDate, cover.EndDate, cover.Type);
        _context.Covers.Add(cover);
        await _context.SaveChangesAsync();
        _auditer.AuditCover(cover.Id, "POST");
        return cover;
    }

    public async Task<bool> DeleteCoverAsync(string id)
    {
        _auditer.AuditCover(id, "DELETE");

        var cover = await GetCoverAsync(id);
        if (cover is null)
        {
            return false;
        }

        _context.Covers.Remove(cover);
        await _context.SaveChangesAsync();
        return true;
    }

    public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
        => _premiumCalculator.Calculate(startDate, endDate, coverType);
}
