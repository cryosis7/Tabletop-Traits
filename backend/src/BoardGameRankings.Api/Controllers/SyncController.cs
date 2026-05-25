using BoardGameRankings.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameRankings.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SyncController : ControllerBase
{
    private readonly ISyncService _syncService;

    public SyncController(ISyncService syncService)
    {
        _syncService = syncService;
    }

    [HttpPost("{username}")]
    public async Task<IActionResult> Sync(string username, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(username))
            return BadRequest("Username is required.");

        var result = await _syncService.SyncUserCollectionAsync(username, cancellationToken);
        return Ok(result);
    }
}
