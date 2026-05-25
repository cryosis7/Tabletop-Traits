using System.Net;
using BoardGameRankings.Domain.Entities;
using BoardGameRankings.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace BoardGameRankings.Infrastructure.BggApi;

public class BggFallbackApiClient : IBggApiClient
{
    private readonly BggXmlApiClient _xmlClient;
    private readonly BggHtmlClient _htmlClient;
    private readonly ILogger<BggFallbackApiClient> _logger;

    public BggFallbackApiClient(
        BggXmlApiClient xmlClient,
        BggHtmlClient htmlClient,
        ILogger<BggFallbackApiClient> logger)
    {
        _xmlClient = xmlClient;
        _htmlClient = htmlClient;
        _logger = logger;
    }

    public async Task<IReadOnlyList<(int GameId, string Name, decimal Rating)>> GetUserRatedCollectionAsync(
        string username, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _xmlClient.GetUserRatedCollectionAsync(username, cancellationToken);
            _logger.LogInformation("BGG collection source: XML API (user: {Username})", username);
            return result;
        }
        catch (HttpRequestException ex) when (IsAuthFailure(ex))
        {
            _logger.LogWarning(ex, "BGG collection XML auth failed, falling back to HTML (user: {Username})", username);
            var result = await _htmlClient.GetUserRatedCollectionAsync(username, cancellationToken);
            _logger.LogInformation("BGG collection source: HTML fallback (user: {Username})", username);
            return result;
        }
    }

    public async Task<IReadOnlyList<BoardGame>> GetGameDetailsAsync(
        IEnumerable<int> gameIds, CancellationToken cancellationToken = default)
    {
        var gameIdList = gameIds.Distinct().ToList();

        try
        {
            var result = await _xmlClient.GetGameDetailsAsync(gameIdList, cancellationToken);
            _logger.LogInformation("BGG game details source: XML API (count: {GameCount})", gameIdList.Count);
            return result;
        }
        catch (HttpRequestException ex) when (IsAuthFailure(ex))
        {
            _logger.LogWarning(ex, "BGG game details XML auth failed, falling back to HTML (count: {GameCount})", gameIdList.Count);
            var result = await _htmlClient.GetGameDetailsAsync(gameIdList, cancellationToken);
            _logger.LogInformation("BGG game details source: HTML fallback (count: {GameCount})", gameIdList.Count);
            return result;
        }
    }

    private static bool IsAuthFailure(HttpRequestException exception)
        => exception.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden;
}
