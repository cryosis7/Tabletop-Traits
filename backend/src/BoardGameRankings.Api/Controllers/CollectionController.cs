using BoardGameRankings.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameRankings.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CollectionController(ICollectionService collectionService) : ControllerBase
{
    [HttpGet("{username}")]
    public async Task<IActionResult> GetCollection(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return BadRequest("Username is required.");

        var collection = await collectionService.GetUserCollectionAsync(username);
        return Ok(collection);
    }
}
