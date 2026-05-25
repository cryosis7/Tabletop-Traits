using BoardGameRankings.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameRankings.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CollectionController : ControllerBase
{
    private readonly ICollectionService _collectionService;

    public CollectionController(ICollectionService collectionService)
    {
        _collectionService = collectionService;
    }

    [HttpGet("{username}")]
    public async Task<IActionResult> GetCollection(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return BadRequest("Username is required.");

        var collection = await _collectionService.GetUserCollectionAsync(username);
        return Ok(collection);
    }
}
