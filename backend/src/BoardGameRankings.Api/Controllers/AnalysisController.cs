using BoardGameRankings.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameRankings.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalysisController(IMechanismAnalysisService analysisService) : ControllerBase
{
    [HttpGet("{username}/mechanisms")]
    public async Task<IActionResult> GetMechanismScores(
        string username,
        [FromQuery] string mode = "average")
    {
        if (string.IsNullOrWhiteSpace(username))
            return BadRequest("Username is required.");

        var scoringMode = mode.ToLowerInvariant() switch
        {
            "cumulative" => ScoringMode.Cumulative,
            _ => ScoringMode.Average
        };

        var scores = await analysisService.GetMechanismScoresAsync(username, scoringMode);
        return Ok(scores);
    }
}
