using BoardGameRankings.Application.DTOs;
using BoardGameRankings.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameRankings.Api.Controllers;

/// <summary>
/// Provides access to mechanism descriptions.
/// </summary>
/// <param name="mechanismDescriptionService">Service that retrieves mechanism descriptions.</param>
[ApiController]
[Route("api/[controller]")]
public class MechanismController(IMechanismDescriptionService mechanismDescriptionService) : ControllerBase
{
    /// <summary>
    /// Retrieves all mechanism descriptions.
    /// </summary>
    /// <returns>A list of all mechanism descriptions.</returns>
    /// <response code="200">Returns the full list of mechanism descriptions.</response>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MechanismDescriptionDto>>> GetAll(CancellationToken cancellationToken)
    {
        var mechanisms = await mechanismDescriptionService.GetAllAsync(cancellationToken);
        return Ok(mechanisms);
    }
}
