using System.Globalization;
using System.Net;
using System.Reflection;
using System.Text.Json;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace BoardGameRankings.DevTools;

public sealed class BggMockServer : IDisposable
{
    private const int BrezmanPageSize = 300;
    private static readonly IReadOnlyList<MockCollectionGame> BrezmanCollection = BuildBrezmanCollection();

    private readonly WireMockServer _server;

    public string Url => _server.Url!;
    public int Port => _server.Port;

    private BggMockServer(WireMockServer server)
    {
        _server = server;
    }

    public static BggMockServer Start(int port = 9090)
    {
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            Port = port
        });

        var instance = new BggMockServer(server);
        instance.ConfigureStubs();
        return instance;
    }

    private void ConfigureStubs()
    {
        ConfigureBrezmanCollectionStubs();

        var collectionHtml = ReadFixture("collection.html");

        // HTML - Collection page
        _server.Given(
            Request.Create()
                .WithPath("/collection/user/*")
                .UsingGet()
        ).RespondWith(
            Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "text/html; charset=utf-8")
                .WithBody(collectionHtml)
        );

        // HTML - Game detail pages (per game ID)
        var gameIds = new[] { "207830", "236457", "368173", "478", "36218" };
        foreach (var gameId in gameIds)
        {
            var gameHtml = ReadFixture($"game_{gameId}.html");
            _server.Given(
                Request.Create()
                    .WithPath($"/boardgame/{gameId}")
                    .UsingGet()
            ).RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "text/html; charset=utf-8")
                    .WithBody(gameHtml)
            );
        }

        foreach (var game in BrezmanCollection)
        {
            var gameHtml = BuildGameDetailsHtml(game);
            _server.Given(
                Request.Create()
                    .WithPath($"/boardgame/{game.Id}")
                    .UsingGet()
            ).RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "text/html; charset=utf-8")
                    .WithBody(gameHtml)
            );
        }
    }

    private void ConfigureBrezmanCollectionStubs()
    {
        ConfigureBrezmanCollectionStubs("Brezman");
        ConfigureBrezmanCollectionStubs("brezman");
    }

    private void ConfigureBrezmanCollectionStubs(string username)
    {
        for (var pageNumber = 1; pageNumber <= 2; pageNumber++)
        {
            var html = BuildCollectionPageHtml(username, pageNumber);

            _server.Given(
                Request.Create()
                    .WithPath($"/collection/user/{username}")
                    .WithParam("pageID", new ExactMatcher(pageNumber.ToString(CultureInfo.InvariantCulture)))
                    .UsingGet()
            ).RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "text/html; charset=utf-8")
                    .WithBody(html)
            );
        }
    }

    private static string ReadFixture(string filename)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"BoardGameRankings.DevTools.Fixtures.{filename}";

        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Fixture not found: {resourceName}");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private static IReadOnlyList<MockCollectionGame> BuildBrezmanCollection()
    {
        var namedGames = new Dictionary<int, (string Name, decimal Rating, int YearPublished)>
        {
            [1] = ("1775: Rebellion", 8.5m, 2013),
            [2] = ("3000 Scoundrels", 5m, 2022),
            [3] = ("7 Wonders", 8m, 2010),
            [301] = ("Star Wars: Escape from Death Star Game", 6m, 1977),
            [302] = ("Star Wars: Outer Rim", 10m, 2019),
            [303] = ("Star Wars: The Deckbuilding Game", 9m, 2023),
            [330] = ("Ticket to Ride", 8m, 2004),
            [360] = ("Wingspan", 8m, 2019),
            [361] = ("The White Castle", 10m, 2023),
            [370] = ("Zooscape", 5m, 2015)
        };

        var mechanismSets = new[]
        {
            new[]
            {
                new MockMechanism(2040, "Hand Management"),
                new MockMechanism(2004, "Set Collection")
            },
            new[]
            {
                new MockMechanism(2082, "Worker Placement"),
                new MockMechanism(2875, "End Game Bonuses")
            },
            new[]
            {
                new MockMechanism(2664, "Deck, Bag, and Pool Building"),
                new MockMechanism(2015, "Variable Player Powers")
            }
        };

        var games = new List<MockCollectionGame>(370);
        for (var index = 1; index <= 370; index++)
        {
            var gameId = 900000 + index;
            var namedGame = namedGames.GetValueOrDefault(index);
            var name = string.IsNullOrWhiteSpace(namedGame.Name)
                ? $"Brezman Mock Game {index:000}"
                : namedGame.Name;
            var rating = namedGame.Rating == 0m
                ? 5m + ((index % 11) * 0.5m)
                : namedGame.Rating;
            var yearPublished = namedGame.YearPublished == 0
                ? 1995 + (index % 30)
                : namedGame.YearPublished;

            games.Add(new MockCollectionGame(
                gameId,
                name,
                rating,
                yearPublished,
                $"https://example.test/brezman/{gameId}.png",
                mechanismSets[(index - 1) % mechanismSets.Length]));
        }

        return games;
    }

    private static string BuildCollectionPageHtml(string username, int pageNumber)
    {
        var totalCount = BrezmanCollection.Count;
        var pageItems = BrezmanCollection
            .Skip((pageNumber - 1) * BrezmanPageSize)
            .Take(BrezmanPageSize)
            .ToList();
        var start = ((pageNumber - 1) * BrezmanPageSize) + 1;
        var end = start + pageItems.Count - 1;

        var pagerLinks = pageNumber switch
        {
            1 => """
                                        <a href="javascript://" onclick="CE_SetPage( 2 );">&raquo;</a>

                <a href="javascript://" onclick="CE_SetPage( 2 );">2</a>
""",
            _ => """
                <a href="javascript://" onclick="CE_SetPage( 1 );">1</a>
                        <a href="javascript://" onclick="CE_SetPage( 1 );">&laquo;</a>
"""
        };

        var rows = string.Join(Environment.NewLine, pageItems.Select(BuildCollectionRowHtml));

        return $"""
<html><body>
<div class='geekcollection_pager'>
  <span class='geekpages'>
    {start.ToString(CultureInfo.InvariantCulture)} to {end.ToString(CultureInfo.InvariantCulture)} of {totalCount.ToString(CultureInfo.InvariantCulture)}


    {pagerLinks}
    Page {pageNumber.ToString(CultureInfo.InvariantCulture)}.
  </span>
</div>
<div>User: {WebUtility.HtmlEncode(username)}</div>
<table>
{rows}
</table>
</body></html>
""";
    }

    private static string BuildCollectionRowHtml(MockCollectionGame game)
    {
        return $"""
  <tr id='row_{game.Id.ToString(CultureInfo.InvariantCulture)}'>
    <td class='collection_objectname'><a href='/boardgame/{game.Id.ToString(CultureInfo.InvariantCulture)}/{Slugify(game.Name)}'>{WebUtility.HtmlEncode(game.Name)}</a></td>
    <td class='collection_rating'><div class='ratingtext'>{game.Rating.ToString(CultureInfo.InvariantCulture)}</div></td>
  </tr>
""";
    }

    private static string BuildGameDetailsHtml(MockCollectionGame game)
    {
        var preload = JsonSerializer.Serialize(new
        {
            item = new
            {
                name = game.Name,
                yearpublished = game.YearPublished.ToString(CultureInfo.InvariantCulture),
                imageurl = game.ThumbnailUrl,
                links = new
                {
                    boardgamemechanic = game.Mechanisms.Select(mechanism => new
                    {
                        name = mechanism.Name,
                        objectid = mechanism.Id.ToString(CultureInfo.InvariantCulture)
                    })
                }
            }
        });

        return $"""
<html><head>
<script>
GEEK.geekitemPreload = {preload};
</script>
</head><body></body></html>
""";
    }

    private static string Slugify(string name)
    {
        var slug = new string(name
            .ToLowerInvariant()
            .Select(character => char.IsLetterOrDigit(character) ? character : '-')
            .ToArray());

        return string.Join('-', slug.Split('-', StringSplitOptions.RemoveEmptyEntries));
    }

    public void Dispose()
    {
        _server.Stop();
        _server.Dispose();
    }

    private sealed record MockCollectionGame(
        int Id,
        string Name,
        decimal Rating,
        int YearPublished,
        string ThumbnailUrl,
        IReadOnlyList<MockMechanism> Mechanisms);

    private sealed record MockMechanism(int Id, string Name);
}
