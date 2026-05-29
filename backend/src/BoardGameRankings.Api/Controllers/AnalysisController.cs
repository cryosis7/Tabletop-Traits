using BoardGameRankings.Application.DTOs;
using BoardGameRankings.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameRankings.Api.Controllers;

/// <summary>
/// Exposes mechanism analysis endpoints for synced user collections.
/// </summary>
/// <param name="analysisService">Application service that calculates mechanism scores.</param>
[ApiController]
[Route("api/[controller]")]
public class AnalysisController(IMechanismAnalysisService analysisService) : ControllerBase
{
    /// <summary>
    /// Retrieves mechanism rankings for a user's synced collection.
    /// All scoring methods are computed and returned; the <paramref name="mode"/> parameter controls sort order only.
    /// </summary>
    /// <remarks>
    /// Supported modes: <c>arithmetic</c>, <c>bayesian</c>, <c>median</c>, <c>trimmed</c>, <c>confidence</c>, <c>positiveRate</c>.
    /// Unknown values fall back to <c>arithmetic</c>.
    /// </remarks>
    /// <param name="username" example="cryosis7">BoardGameGeek username whose synced collection should be analyzed.</param>
    /// <param name="mode" example="arithmetic">Which scoring method to sort results by.</param>
    /// <returns>A list of mechanism score summaries for the user's collection.</returns>
    /// <response code="200">Returns the calculated mechanism scores.</response>
    /// <response code="400">Returned when the username is missing or whitespace.</response>
    [HttpGet("{username}/mechanisms")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<MechanismScoreDto>>> GetMechanismScores(
        string username,
        [FromQuery] string mode = "arithmetic")
    {
        if (string.IsNullOrWhiteSpace(username))
            return BadRequest("Username is required.");

        var scoringMode = mode.ToLowerInvariant() switch
        {
            "bayesian" => ScoringMode.Bayesian,
            "median" => ScoringMode.Median,
            "trimmed" => ScoringMode.Trimmed,
            "confidence" => ScoringMode.Confidence,
            "positiverate" => ScoringMode.PositiveRate,
            _ => ScoringMode.Arithmetic
        };

        var scores = await analysisService.GetMechanismScoresAsync(username, scoringMode);
        return Ok(scores);
    }
}
