using System.Net;
using System.Xml.Linq;
using BoardGameRankings.Domain.Entities;
using BoardGameRankings.Domain.Interfaces;

namespace BoardGameRankings.Infrastructure.BggApi;

public class BggApiClient : IBggApiClient
{
    private const string BaseUrl = "https://boardgamegeek.com/xmlapi2";
    private const int BatchSize = 20;
    private const int RetryDelayMs = 3000;
    private const int MaxRetries = 10;

    private readonly HttpClient _httpClient;
    private readonly SemaphoreSlim _rateLimiter = new(1, 1);
    private DateTime _lastRequestTime = DateTime.MinValue;

    public BggApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<(int GameId, string Name, decimal Rating)>> GetUserRatedCollectionAsync(
        string username, CancellationToken cancellationToken = default)
    {
        var url = $"{BaseUrl}/collection?username={Uri.EscapeDataString(username)}&rated=1&subtype=boardgame&stats=1";

        var xml = await GetWithRetryAsync(url, cancellationToken);
        var doc = XDocument.Parse(xml);

        var items = doc.Descendants("item")
            .Select(item =>
            {
                var gameId = int.Parse(item.Attribute("objectid")!.Value);
                var name = item.Element("name")?.Value ?? "Unknown";
                var ratingStr = item.Descendants("rating")
                    .FirstOrDefault()?.Attribute("value")?.Value;

                if (ratingStr == null || !decimal.TryParse(ratingStr, out var rating))
                    return (GameId: gameId, Name: name, Rating: 0m);

                return (GameId: gameId, Name: name, Rating: rating);
            })
            .Where(x => x.Rating > 0)
            .ToList();

        return items;
    }

    public async Task<IReadOnlyList<BoardGame>> GetGameDetailsAsync(
        IEnumerable<int> gameIds, CancellationToken cancellationToken = default)
    {
        var allGames = new List<BoardGame>();
        var idBatches = gameIds
            .Select((id, index) => new { id, index })
            .GroupBy(x => x.index / BatchSize)
            .Select(g => g.Select(x => x.id).ToList());

        foreach (var batch in idBatches)
        {
            var idsCsv = string.Join(",", batch);
            var url = $"{BaseUrl}/thing?id={idsCsv}&type=boardgame";

            var xml = await GetWithRetryAsync(url, cancellationToken);
            var doc = XDocument.Parse(xml);

            var games = doc.Descendants("item").Select(item =>
            {
                var id = int.Parse(item.Attribute("id")!.Value);
                var name = item.Elements("name")
                    .FirstOrDefault(n => n.Attribute("type")?.Value == "primary")
                    ?.Attribute("value")?.Value ?? "Unknown";
                var yearStr = item.Element("yearpublished")?.Attribute("value")?.Value;
                int? year = yearStr != null && int.TryParse(yearStr, out var y) ? y : null;
                var thumbnail = item.Element("thumbnail")?.Value;

                var mechanisms = item.Elements("link")
                    .Where(l => l.Attribute("type")?.Value == "boardgamemechanic")
                    .Select(l => new Mechanism(
                        int.Parse(l.Attribute("id")!.Value),
                        l.Attribute("value")!.Value))
                    .ToList();

                return new BoardGame(id, name, year, thumbnail, mechanisms);
            }).ToList();

            allGames.AddRange(games);
        }

        return allGames;
    }

    private async Task<string> GetWithRetryAsync(string url, CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < MaxRetries; attempt++)
        {
            await RateLimitAsync(cancellationToken);

            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Accepted)
            {
                // BGG queues the request - wait and retry
                await Task.Delay(RetryDelayMs, cancellationToken);
                continue;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(cancellationToken);
        }

        throw new HttpRequestException($"BGG API did not return data after {MaxRetries} retries for URL: {url}");
    }

    private async Task RateLimitAsync(CancellationToken cancellationToken)
    {
        await _rateLimiter.WaitAsync(cancellationToken);
        try
        {
            var elapsed = DateTime.UtcNow - _lastRequestTime;
            if (elapsed.TotalMilliseconds < 1100)
            {
                var delay = 1100 - (int)elapsed.TotalMilliseconds;
                await Task.Delay(delay, cancellationToken);
            }
            _lastRequestTime = DateTime.UtcNow;
        }
        finally
        {
            _rateLimiter.Release();
        }
    }
}
