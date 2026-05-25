using BoardGameRankings.Application.DTOs;
using BoardGameRankings.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameRankings.Api.Controllers;

/// <summary>
/// Exposes read-only access to a user's stored rated collection.
/// </summary>
/// <param name="collectionService">Application service that loads stored collection data.</param>
[ApiController]
[Route("api/[controller]")]
public class CollectionController(ICollectionService collectionService) : ControllerBase
{
    /// <summary>
    /// Retrieves the stored rated collection for a user.
    /// </summary>
    /// <remarks>
    /// Only games with a user rating are returned. Run the sync endpoint first if no local data has been stored yet.
    /// </remarks>
    /// <param name="username" example="cryosis7">BoardGameGeek username whose stored collection should be returned.</param>
    /// <returns>The user's rated games, including mechanisms and thumbnail metadata.</returns>
    /// <response code="200">Returns the user's stored rated collection.</response>
    /// <response code="400">Returned when the username is missing or whitespace.</response>
    [HttpGet("{username}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<BoardGameDto>>> GetCollection(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return BadRequest("Username is required.");

        var collection = await collectionService.GetUserCollectionAsync(username);
        return Ok(collection);
    }
}
