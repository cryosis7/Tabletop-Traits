using System.Globalization;
using System.Net;
using System.Xml.Linq;
using BoardGameRankings.Domain.Entities;
using BoardGameRankings.Domain.Interfaces;

namespace BoardGameRankings.Infrastructure.BggApi;

public class BggXmlApiClient(HttpClient httpClient) : IBggApiClient
{
    private const int MaxIdsPerRequest = 20;
    private const int MaxRetryAttempts = 5;
    private const int MaxConcurrentRequests = 5;
    private static readonly TimeSpan InitialRetryDelay = TimeSpan.FromSeconds(2);

    private readonly SemaphoreSlim _concurrencyLimiter = new(MaxConcurrentRequests, MaxConcurrentRequests);

    public async Task<IReadOnlyList<(int GameId, string Name, decimal Rating)>> GetUserRatedCollectionAsync(
        string username, CancellationToken cancellationToken = default)
    {
        var url = $"/xmlapi2/collection?username={Uri.EscapeDataString(username)}&rated=1&subtype=boardgame&stats=1";
        var xml = await GetXmlWithRetryAsync(url, cancellationToken);
        return ParseCollectionXml(xml);
    }

    public async Task<IReadOnlyList<BoardGame>> GetGameDetailsAsync(
        IEnumerable<int> gameIds, CancellationToken cancellationToken = default)
    {
        var ids = gameIds.Distinct().ToList();

        var tasks = ids.Chunk(MaxIdsPerRequest).Select(async batch =>
        {
            var batchIds = new HashSet<int>(batch);
            var idsParam = string.Join(",", batch);
            var url = $"/xmlapi2/thing?id={idsParam}&type=boardgame";
            var xml = await GetXmlWithRetryAsync(url, cancellationToken);
            return ParseThingXml(xml).Where(g => batchIds.Contains(g.Id));
        });

        var results = await Task.WhenAll(tasks);
        return results.SelectMany(g => g).ToList();
    }

    private static IReadOnlyList<(int GameId, string Name, decimal Rating)> ParseCollectionXml(string xml)
    {
        var doc = XDocument.Parse(xml);
        var items = new List<(int GameId, string Name, decimal Rating)>();

        foreach (var item in doc.Descendants("item"))
        {
            var objectId = item.Attribute("objectid")?.Value;
            if (!int.TryParse(objectId, out var gameId))
            {
                continue;
            }

            var name = item.Element("name")?.Value ?? "Unknown";

            var ratingElement = item.Descendants("rating").FirstOrDefault();
            var ratingValue = ratingElement?.Attribute("value")?.Value;
            if (ratingValue == null || !decimal.TryParse(ratingValue, NumberStyles.Number, CultureInfo.InvariantCulture, out var rating))
            {
                continue;
            }

            items.Add((gameId, name, rating));
        }

        return items;
    }

    private static IReadOnlyList<BoardGame> ParseThingXml(string xml)
    {
        var doc = XDocument.Parse(xml);
        var games = new List<BoardGame>();

        foreach (var item in doc.Descendants("item"))
        {
            var idAttr = item.Attribute("id")?.Value;
            if (!int.TryParse(idAttr, out var gameId))
            {
                continue;
            }

            var name = item.Elements("name")
                .FirstOrDefault(n => n.Attribute("type")?.Value == "primary")
                ?.Attribute("value")?.Value ?? "Unknown";

            int? yearPublished = null;
            var yearValue = item.Element("yearpublished")?.Attribute("value")?.Value;
            if (int.TryParse(yearValue, out var parsedYear))
            {
                yearPublished = parsedYear;
            }

            var thumbnail = item.Element("thumbnail")?.Value;

            var mechanisms = item.Elements("link")
                .Where(l => l.Attribute("type")?.Value == "boardgamemechanic")
                .Select(l =>
                {
                    var mechId = l.Attribute("id")?.Value;
                    var mechName = l.Attribute("value")?.Value;
                    if (mechId == null || mechName == null || !int.TryParse(mechId, out var id))
                    {
                        return null;
                    }
                    return new Mechanism(id, mechName);
                })
                .Where(m => m != null)
                .Cast<Mechanism>()
                .ToList();

            games.Add(new BoardGame(gameId, name, yearPublished, thumbnail, mechanisms));
        }

        return games;
    }

    private async Task<string> GetXmlWithRetryAsync(string url, CancellationToken cancellationToken)
    {
        await _concurrencyLimiter.WaitAsync(cancellationToken);
        try
        {
            var delay = InitialRetryDelay;

            for (var attempt = 0; attempt <= MaxRetryAttempts; attempt++)
            {
                var response = await httpClient.GetAsync(url, cancellationToken);

                if (response.StatusCode == HttpStatusCode.Accepted)
                {
                    if (attempt == MaxRetryAttempts)
                    {
                        throw new HttpRequestException(
                            $"BGG API returned 202 (Accepted) after {MaxRetryAttempts} retries. The request is still being processed.");
                    }

                    await Task.Delay(delay, cancellationToken);
                    delay *= 2;
                    continue;
                }

                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    if (attempt == MaxRetryAttempts)
                    {
                        response.EnsureSuccessStatusCode();
                    }

                    var retryAfter = response.Headers.RetryAfter?.Delta ?? delay;
                    await Task.Delay(retryAfter, cancellationToken);
                    delay *= 2;
                    continue;
                }

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync(cancellationToken);
            }

            throw new HttpRequestException("Exhausted retry attempts for BGG XML API request.");
        }
        finally
        {
            _concurrencyLimiter.Release();
        }
    }
}
