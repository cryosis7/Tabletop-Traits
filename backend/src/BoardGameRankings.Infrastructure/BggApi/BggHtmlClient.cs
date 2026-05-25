using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using BoardGameRankings.Domain.Entities;
using BoardGameRankings.Domain.Interfaces;
using HtmlAgilityPack;

namespace BoardGameRankings.Infrastructure.BggApi;

public class BggHtmlClient(HttpClient httpClient) : IBggApiClient
{
    private static readonly Regex CollectionSummaryRegex = new(@"(?<start>[\d,]+)\s+to\s+(?<end>[\d,]+)\s+of\s+(?<total>[\d,]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex CollectionPagerRegex = new(@"CE_SetPage\(\s*(?<page>\d+)\s*\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex GameHrefRegex = new(@"/boardgame/(?<id>\d+)/", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex GeekItemPreloadRegex = new(@"GEEK\.geekitemPreload = (?<json>\{.*?\});", RegexOptions.Compiled | RegexOptions.Singleline);

    private readonly SemaphoreSlim _rateLimiter = new(1, 1);
    private DateTime _lastRequestTime = DateTime.MinValue;

    public async Task<IReadOnlyList<(int GameId, string Name, decimal Rating)>> GetUserRatedCollectionAsync(
        string username, CancellationToken cancellationToken = default)
    {
        var items = new List<(int GameId, string Name, decimal Rating)>();
        var visitedPages = new HashSet<int>();
        int? pageNumber = 1;

        while (pageNumber.HasValue && visitedPages.Add(pageNumber.Value))
        {
            var currentPageNumber = pageNumber.Value;
            var url = $"/collection/user/{Uri.EscapeDataString(username)}?rated=1&subtype=boardgame&all=1&pageID={currentPageNumber}";
            var html = await GetHtmlAsync(url, cancellationToken);
            var parsedPage = ParseCollectionHtmlPage(html, currentPageNumber);

            items.AddRange(parsedPage.Items);
            pageNumber = parsedPage.NextPageNumber;
        }

        return items
            .GroupBy(item => item.GameId)
            .Select(group => group.First())
            .ToList();
    }

    public async Task<IReadOnlyList<BoardGame>> GetGameDetailsAsync(
        IEnumerable<int> gameIds, CancellationToken cancellationToken = default)
    {
        var ids = gameIds.Distinct().ToList();
        var games = new List<BoardGame>(ids.Count);

        foreach (var gameId in ids)
        {
            var url = $"/boardgame/{gameId}";
            var html = await GetHtmlAsync(url, cancellationToken);
            games.Add(ParseGameDetailsHtmlPage(gameId, html));
        }

        return games;
    }

    private static (IReadOnlyList<(int GameId, string Name, decimal Rating)> Items, int? NextPageNumber) ParseCollectionHtmlPage(
        string html,
        int currentPageNumber)
    {
        var document = new HtmlDocument();
        document.LoadHtml(html);

        var items = new List<(int GameId, string Name, decimal Rating)>();
        var rows = document.DocumentNode.SelectNodes("//tr[starts-with(@id,'row_')]");

        if (rows != null)
        {
            foreach (var row in rows)
            {
                var linkNode = row.SelectSingleNode(".//td[contains(@class,'collection_objectname')]//a[contains(@href,'/boardgame/')]");
                var ratingNode = row.SelectSingleNode(".//td[contains(@class,'collection_rating')]//div[contains(@class,'ratingtext')]");

                if (linkNode == null || ratingNode == null)
                {
                    continue;
                }

                var href = linkNode.GetAttributeValue("href", string.Empty);
                var match = GameHrefRegex.Match(href);
                if (!match.Success)
                {
                    continue;
                }

                if (!int.TryParse(match.Groups["id"].Value, out var gameId))
                {
                    continue;
                }

                var name = HtmlEntity.DeEntitize(linkNode.InnerText).Trim();
                var ratingText = HtmlEntity.DeEntitize(ratingNode.InnerText).Trim();
                if (!decimal.TryParse(ratingText, NumberStyles.Number, CultureInfo.InvariantCulture, out var rating))
                {
                    continue;
                }

                items.Add((gameId, name, rating));
            }
        }

        int? nextPageNumber = null;
        var pagerLinks = document.DocumentNode.SelectNodes("//div[contains(@class,'geekcollection_pager')]//a[contains(@onclick,'CE_SetPage')]");
        if (pagerLinks != null)
        {
            nextPageNumber = pagerLinks
                .Select(link => CollectionPagerRegex.Match(link.GetAttributeValue("onclick", string.Empty)))
                .Where(match => match.Success && int.TryParse(match.Groups["page"].Value, out _))
                .Select(match => int.Parse(match.Groups["page"].Value, CultureInfo.InvariantCulture))
                .Where(page => page > currentPageNumber)
                .DefaultIfEmpty()
                .Min();

            if (nextPageNumber == 0)
            {
                nextPageNumber = null;
            }
        }

        var summaryMatch = CollectionSummaryRegex.Match(document.DocumentNode.InnerText);
        if (summaryMatch.Success
            && TryParseCollectionNumber(summaryMatch.Groups["end"].Value, out var end)
            && TryParseCollectionNumber(summaryMatch.Groups["total"].Value, out var total)
            && end < total)
        {
            if (!nextPageNumber.HasValue)
            {
                nextPageNumber = currentPageNumber + 1;
            }
        }

        return (items, nextPageNumber);
    }

    private static bool TryParseCollectionNumber(string value, out int parsedValue)
    {
        return int.TryParse(
            value.Replace(",", string.Empty),
            NumberStyles.None,
            CultureInfo.InvariantCulture,
            out parsedValue);
    }

    private static BoardGame ParseGameDetailsHtmlPage(int gameId, string html)
    {
        var preloadMatch = GeekItemPreloadRegex.Match(html);
        if (!preloadMatch.Success)
        {
            throw new InvalidOperationException($"Unable to parse BGG game page for game {gameId}.");
        }

        using var document = JsonDocument.Parse(preloadMatch.Groups["json"].Value);
        var item = document.RootElement.GetProperty("item");

        var name = item.TryGetProperty("name", out var nameProperty)
            ? nameProperty.GetString() ?? "Unknown"
            : "Unknown";

        int? year = null;
        if (item.TryGetProperty("yearpublished", out var yearProperty)
            && int.TryParse(yearProperty.GetString(), out var parsedYear))
        {
            year = parsedYear;
        }

        var thumbnail = item.TryGetProperty("imageurl", out var imageProperty)
            ? imageProperty.GetString()
            : null;

        var mechanisms = new List<Mechanism>();
        if (item.TryGetProperty("links", out var linksProperty)
            && linksProperty.TryGetProperty("boardgamemechanic", out var mechanismsProperty)
            && mechanismsProperty.ValueKind == JsonValueKind.Array)
        {
            foreach (var mechanismProperty in mechanismsProperty.EnumerateArray())
            {
                if (!mechanismProperty.TryGetProperty("objectid", out var mechanismIdProperty)
                    || !int.TryParse(mechanismIdProperty.GetString(), out var mechanismId)
                    || !mechanismProperty.TryGetProperty("name", out var mechanismNameProperty))
                {
                    continue;
                }

                var mechanismName = mechanismNameProperty.GetString();
                if (string.IsNullOrWhiteSpace(mechanismName))
                {
                    continue;
                }

                mechanisms.Add(new Mechanism(mechanismId, mechanismName));
            }
        }

        return new BoardGame(gameId, name, year, thumbnail, mechanisms);
    }

    private async Task<string> GetHtmlAsync(string url, CancellationToken cancellationToken)
    {
        await RateLimitAsync(cancellationToken);

        var response = await httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    private async Task RateLimitAsync(CancellationToken cancellationToken)
    {
        if (httpClient.BaseAddress?.IsLoopback == true)
        {
            return;
        }

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
