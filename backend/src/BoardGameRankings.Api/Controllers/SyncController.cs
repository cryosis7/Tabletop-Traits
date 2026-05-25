using BoardGameRankings.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameRankings.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SyncController(ISyncService syncService) : ControllerBase
{
    [HttpPost("{username}")]
    public async Task<IActionResult> Sync(string username, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(username))
            return BadRequest("Username is required.");

        var result = await syncService.SyncUserCollectionAsync(username, cancellationToken);
        return Ok(result);
    }
}
