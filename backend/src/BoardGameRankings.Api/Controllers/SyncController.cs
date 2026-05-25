using BoardGameRankings.Application.DTOs;
using BoardGameRankings.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameRankings.Api.Controllers;

/// <summary>
/// Exposes synchronization endpoints for importing BoardGameGeek collection data.
/// </summary>
/// <param name="syncService">Application service that synchronizes collection data.</param>
[ApiController]
[Route("api/[controller]")]
public class SyncController(ISyncService syncService) : ControllerBase
{
    /// <summary>
    /// Synchronizes a user's rated games from BoardGameGeek into local storage.
    /// </summary>
    /// <remarks>
    /// This refreshes the locally cached collection and mechanism data used by the collection and analysis endpoints.
    /// </remarks>
    /// <param name="username" example="cryosis7">BoardGameGeek username to synchronize.</param>
    /// <param name="cancellationToken">Cancels the sync request.</param>
    /// <returns>The current sync status, including progress counts and the last successful sync time.</returns>
    /// <response code="200">Returns the sync result after the user's collection has been refreshed.</response>
    /// <response code="400">Returned when the username is missing or whitespace.</response>
    [HttpPost("{username}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SyncStatusDto>> Sync(string username, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(username))
            return BadRequest("Username is required.");

        var result = await syncService.SyncUserCollectionAsync(username, cancellationToken);
        return Ok(result);
    }
}
