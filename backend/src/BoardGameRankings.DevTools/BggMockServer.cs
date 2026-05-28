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
        ConfigureXmlApiStubs();

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

    private void ConfigureXmlApiStubs()
    {
        // XML API2 - Collection for testuser
        var testUserCollectionXml = BuildCollectionXml(new[]
        {
            (207830, "5-Minute Dungeon", 8m),
            (236457, "Architects of the West Kingdom", 7m),
            (368173, "Dune: Imperium", 9m),
            (478, "Citadels", 7m),
            (36218, "Dominion", 5m)
        });

        _server.Given(
            Request.Create()
                .WithPath("/xmlapi2/collection")
                .WithParam("username", new ExactMatcher("testuser"))
                .UsingGet()
        ).RespondWith(
            Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/xml; charset=utf-8")
                .WithBody(testUserCollectionXml)
        );

        // XML API2 - Collection for Cryosis7
        var cryosis7CollectionXml = BuildCollectionXml(new[]
        {
            (207830, "5-Minute Dungeon", 8m),
            (140934, "Arboretum", 7m),
            (236457, "Architects of the West Kingdom", 7m),
            (822, "Carcassonne", 7m),
            (478, "Citadels", 7m),
            (36218, "Dominion", 5m),
            (70323, "King of Tokyo", 8m),
            (368173, "Dune: Imperium", 9m),
            (129622, "Love Letter", 6m),
            (1927, "Munchkin", 4m),
            (403376, "One Last Score", 5m),
            (30549, "Pandemic", 5m),
            (244521, "The Quacks of Quedlinburg", 8m),
            (2381, "Scattergories", 8m),
            (373106, "Sky Team", 8m),
            (34635, "Stone Age", 8m),
            (192291, "Sushi Go Party!", 7m),
            (133473, "Sushi Go!", 7m),
            (218530, "Tortuga 1667", 6m)
        });

        _server.Given(
            Request.Create()
                .WithPath("/xmlapi2/collection")
                .WithParam("username", new WildcardMatcher("*ryosis*", true))
                .UsingGet()
        ).RespondWith(
            Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/xml; charset=utf-8")
                .WithBody(cryosis7CollectionXml)
        );

        // XML API2 - Collection for Brezman
        var brezmanCollectionXml = BuildCollectionXml(
            BrezmanCollection.Select(g => (g.Id, g.Name, g.Rating)).ToArray());

        _server.Given(
            Request.Create()
                .WithPath("/xmlapi2/collection")
                .WithParam("username", new WildcardMatcher("*rezman*", true))
                .UsingGet()
        ).RespondWith(
            Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/xml; charset=utf-8")
                .WithBody(brezmanCollectionXml)
        );

        // XML API2 - Thing details (match any comma-separated IDs)
        var allGames = new Dictionary<int, (string Name, int Year, string Thumbnail, (int Id, string Name)[] Mechanisms)>
        {
            [207830] = ("5-Minute Dungeon", 2017, "https://cf.geekdo-images.com/bFZKvxPJ23dzGwgpOg-mIA__thumb/img/example.png",
                new[] { (2857, "Card Play Conflict Resolution"), (2023, "Cooperative Game"), (2040, "Hand Management"), (2831, "Real-Time") }),
            [236457] = ("Architects of the West Kingdom", 2018, "https://cf.geekdo-images.com/OAX7HfOz-9N60StgADzd0g__thumb/img/example.png",
                new[] { (2082, "Worker Placement"), (2912, "Contracts"), (2875, "End Game Bonuses") }),
            [368173] = ("Dune: Imperium", 2020, "https://cf.geekdo-images.com/PhjygpWSo-0labGrPBMyyg__thumb/img/example.png",
                new[] { (2664, "Deck, Bag, and Pool Building"), (2082, "Worker Placement"), (2875, "End Game Bonuses") }),
            [478] = ("Citadels", 2000, "https://cf.geekdo-images.com/42iOptCcfGiIl2bpDIx7Bw__thumb/img/example.png",
                new[] { (2040, "Hand Management"), (2015, "Variable Player Powers"), (2686, "Take That") }),
            [36218] = ("Dominion", 2008, "https://cf.geekdo-images.com/j6iQpZ4HkVOHRCKrZUB0bg__thumb/img/example.png",
                new[] { (2664, "Deck, Bag, and Pool Building"), (2040, "Hand Management") }),
            [140934] = ("Arboretum", 2015, "https://cf.geekdo-images.com/XYOn10oXBrDqHySf0jvnyQ__thumb/img/example.png",
                new[] { (2883, "Connections"), (2040, "Hand Management"), (2962, "Move Through Deck"), (2041, "Open Drafting"), (3101, "Ordering"), (2048, "Pattern Building") }),
            [822] = ("Carcassonne", 2000, "https://cf.geekdo-images.com/peUgu3A20LRmAXAMyDQfpQ__thumb/img/example.png",
                new[] { (2080, "Area Majority / Influence"), (2043, "Enclosure"), (2871, "Kill Steal"), (2959, "Map Addition"), (2048, "Pattern Building"), (2940, "Square Grid") }),
            [70323] = ("King of Tokyo", 2011, "https://cf.geekdo-images.com/m_RzXpHURC0_xLkvRSR_sw__thumb/img/example.png",
                new[] { (2072, "Dice Rolling"), (2856, "Die Icon Resolution"), (2886, "King of the Hill"), (2041, "Open Drafting"), (2685, "Player Elimination"), (2661, "Push Your Luck") }),
            [129622] = ("Love Letter", 2012, "https://cf.geekdo-images.com/T1ltXwapFUtghS9A7_tf4g__thumb/img/example.png",
                new[] { (3002, "Deduction"), (2040, "Hand Management"), (2685, "Player Elimination"), (2823, "Score-and-Reset Game"), (2686, "Take That") }),
            [1927] = ("Munchkin", 2001, "https://cf.geekdo-images.com/J-ts3MW0UhDzs621TR6cog__thumb/img/example.png",
                new[] { (2040, "Hand Management"), (2876, "Race"), (2686, "Take That"), (2015, "Variable Player Powers") }),
            [403376] = ("One Last Score", 2023, "https://cf.geekdo-images.com/TASGNGGUN7ZeexBudn2TUA__thumb/img/example.png",
                new[] { (2857, "Card Play Conflict Resolution"), (2837, "Interrupts"), (3007, "Matching"), (3099, "Multi-Use Cards"), (2686, "Take That") }),
            [30549] = ("Pandemic", 2008, "https://cf.geekdo-images.com/S3ybV1LAp-8SnHIXLLjVqA__thumb/img/example.png",
                new[] { (2001, "Action Points"), (2956, "Chaining"), (2912, "Contracts"), (2023, "Cooperative Game"), (2850, "Events"), (2040, "Hand Management") }),
            [244521] = ("The Quacks of Quedlinburg", 2018, "https://cf.geekdo-images.com/B1bLRWzTASZ-xx9NoAE79A__thumb/img/example.png",
                new[] { (2887, "Catch the Leader"), (2664, "Deck, Bag, and Pool Building"), (2901, "Delayed Purchase"), (2072, "Dice Rolling"), (2856, "Die Icon Resolution"), (2850, "Events") }),
            [2381] = ("Scattergories", 1988, "https://cf.geekdo-images.com/eIL4hvMb7ZPgizc7BZOh-g__thumb/img/example.png",
                new[] { (2072, "Dice Rolling"), (2055, "Paper-and-Pencil") }),
            [373106] = ("Sky Team", 2023, "https://cf.geekdo-images.com/uXMeQzNenHb3zK7Hoa6b2w__thumb/img/example.png",
                new[] { (2893, "Communication Limits"), (2023, "Cooperative Game"), (2072, "Dice Rolling"), (2822, "Scenario / Mission / Campaign Game"), (2828, "Turn Order: Progressive"), (2015, "Variable Player Powers") }),
            [34635] = ("Stone Age", 2008, "https://cf.geekdo-images.com/elmZegVZ6gp4_5izUgxGQQ__thumb/img/example.png",
                new[] { (2912, "Contracts"), (2072, "Dice Rolling"), (2875, "End Game Bonuses"), (2004, "Set Collection"), (2828, "Turn Order: Progressive"), (2082, "Worker Placement") }),
            [192291] = ("Sushi Go Party!", 2016, "https://cf.geekdo-images.com/2f9uTicUSXkdPp2Yks6zFw__thumb/img/example.png",
                new[] { (2984, "Closed Drafting"), (2875, "End Game Bonuses"), (2040, "Hand Management"), (2004, "Set Collection"), (2020, "Simultaneous Action Selection"), (2897, "Variable Set-up") }),
            [133473] = ("Sushi Go!", 2013, "https://cf.geekdo-images.com/Fn3PSPZVxa3YurlorITQ1Q__thumb/img/example.png",
                new[] { (2984, "Closed Drafting"), (2875, "End Game Bonuses"), (2040, "Hand Management"), (2823, "Score-and-Reset Game"), (2004, "Set Collection"), (2020, "Simultaneous Action Selection") }),
            [218530] = ("Tortuga 1667", 2017, "https://cf.geekdo-images.com/rT6zVN1zRbEMHok5V_zoGQ__thumb/img/example.png",
                new[] { (2046, "Area Movement"), (2040, "Hand Management"), (2891, "Hidden Roles"), (2019, "Team-Based Game"), (2017, "Voting") })
        };

        // Add Brezman games to the thing lookup
        foreach (var game in BrezmanCollection)
        {
            allGames[game.Id] = (game.Name, game.YearPublished, game.ThumbnailUrl,
                game.Mechanisms.Select(m => (m.Id, m.Name)).ToArray());
        }

        // XML API2 - Thing details (catch-all that returns all known games matching requested IDs)
        // We register individual game XMLs and a catch-all for any id param
        var allGamesXml = BuildThingXml(allGames.Keys, allGames);

        _server.Given(
            Request.Create()
                .WithPath("/xmlapi2/thing")
                .UsingGet()
        ).RespondWith(
            Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/xml; charset=utf-8")
                .WithBody(allGamesXml)
        );
    }

    private static string BuildCollectionXml(IReadOnlyList<(int Id, string Name, decimal Rating)> items)
    {
        var itemElements = string.Join(Environment.NewLine, items.Select(i =>
            $"""
    <item objecttype="thing" objectid="{i.Id}" subtype="boardgame">
        <name sortindex="1">{WebUtility.HtmlEncode(i.Name)}</name>
        <status own="0" prevowned="0" fortrade="0" want="0" wanttoplay="0" wanttobuy="0" wishlist="0" preordered="0" lastmodified="2024-01-01 00:00:00"/>
        <stats minplayers="1" maxplayers="4" minplaytime="30" maxplaytime="60" playingtime="60" numowned="1000">
            <rating value="{i.Rating.ToString(CultureInfo.InvariantCulture)}">
                <usersrated value="1000"/>
                <average value="7.5"/>
                <bayesaverage value="7.2"/>
            </rating>
        </stats>
    </item>
"""));

        return $"""
<?xml version="1.0" encoding="utf-8"?>
<items totalitems="{items.Count}" termsofuse="https://boardgamegeek.com/xmlapi/termsofuse" pubdate="Wed, 01 Jan 2025 00:00:00 +0000">
{itemElements}
</items>
""";
    }

    private static string BuildThingXml(IEnumerable<int> ids, Dictionary<int, (string Name, int Year, string Thumbnail, (int Id, string Name)[] Mechanisms)> allGames)
    {
        var itemElements = string.Join(Environment.NewLine, ids
            .Where(allGames.ContainsKey)
            .Select(id =>
            {
                var game = allGames[id];
                var links = string.Join(Environment.NewLine, game.Mechanisms.Select(m =>
                    $"""        <link type="boardgamemechanic" id="{m.Id}" value="{WebUtility.HtmlEncode(m.Name)}"/>"""));

                return $"""
    <item type="boardgame" id="{id}">
        <thumbnail>{WebUtility.HtmlEncode(game.Thumbnail)}</thumbnail>
        <name type="primary" sortindex="1" value="{WebUtility.HtmlEncode(game.Name)}"/>
        <yearpublished value="{game.Year}"/>
{links}
    </item>
""";
            }));

        return $"""
<?xml version="1.0" encoding="utf-8"?>
<items termsofuse="https://boardgamegeek.com/xmlapi/termsofuse">
{itemElements}
</items>
""";
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
