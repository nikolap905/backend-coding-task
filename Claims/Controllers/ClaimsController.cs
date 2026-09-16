using Claims.Models;
using Claims.Services;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class ClaimsController : ControllerBase
{
    private readonly IClaimsService _claimsService;
    private readonly ILogger<ClaimsController> _logger;

    public ClaimsController(IClaimsService claimsService, ILogger<ClaimsController> logger)
    {
        _claimsService = claimsService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IEnumerable<Claim>> GetAsync()
    {
        return await _claimsService.GetClaimsAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Claim>> GetAsync(string id)
    {
        var claim = await _claimsService.GetClaimAsync(id);
        return claim is null ? NotFound() : Ok(claim);
    }

    [HttpPost]
    public async Task<ActionResult<Claim>> CreateAsync(Claim claim)
    {
        var (created, error) = await _claimsService.CreateClaimAsync(claim);
        return error is not null ? BadRequest(error) : Ok(created);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(string id)
    {
        var deleted = await _claimsService.DeleteClaimAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
