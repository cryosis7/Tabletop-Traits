using System.Reflection;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace BoardGameRankings.DevTools;

public sealed class BggMockServer : IDisposable
{
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
        var collectionXml = ReadFixture("collection.xml");
        var thingsXml = ReadFixture("things.xml");

        // XML API - Collection endpoint
        _server.Given(
            Request.Create()
                .WithPath("/xmlapi2/collection")
                .UsingGet()
        ).RespondWith(
            Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/xml; charset=utf-8")
                .WithBody(collectionXml)
        );

        // XML API - Thing endpoint (game details)
        _server.Given(
            Request.Create()
                .WithPath("/xmlapi2/thing")
                .UsingGet()
        ).RespondWith(
            Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/xml; charset=utf-8")
                .WithBody(thingsXml)
        );

        // HTML fallback - Collection page (returns 401 to trigger fallback testing if desired,
        // or return valid HTML for direct HTML client testing)
        _server.Given(
            Request.Create()
                .WithPath("/collection/user/*")
                .UsingGet()
        ).RespondWith(
            Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "text/html; charset=utf-8")
                .WithBody(BuildCollectionHtml())
        );

        // HTML fallback - Game detail page
        _server.Given(
            Request.Create()
                .WithPath("/boardgame/*")
                .UsingGet()
        ).RespondWith(
            Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "text/html; charset=utf-8")
                .WithBody(BuildGameDetailHtml())
        );
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

    private static string BuildCollectionHtml()
    {
        return """
            <html><body>
            <div class="geekpages">
              <span>1 to 5 of 5 results</span>
            </div>
            <table>
              <tr id="row_1"><td class="collection_objectname">
                <a href="/boardgame/207830/5-minute-dungeon">5-Minute Dungeon</a>
              </td><td class="collection_rating">8</td></tr>
              <tr id="row_2"><td class="collection_objectname">
                <a href="/boardgame/236457/architects-west-kingdom">Architects of the West Kingdom</a>
              </td><td class="collection_rating">7</td></tr>
              <tr id="row_3"><td class="collection_objectname">
                <a href="/boardgame/368173/dune-imperium">Dune: Imperium</a>
              </td><td class="collection_rating">9</td></tr>
              <tr id="row_4"><td class="collection_objectname">
                <a href="/boardgame/478/citadels">Citadels</a>
              </td><td class="collection_rating">7</td></tr>
              <tr id="row_5"><td class="collection_objectname">
                <a href="/boardgame/36218/dominion">Dominion</a>
              </td><td class="collection_rating">5</td></tr>
            </table>
            </body></html>
            """;
    }

    private static string BuildGameDetailHtml()
    {
        return """
            <html><head>
            <script>
            GEEK.geekitemPreload = {"item":{"name":"Mock Game","yearpublished":"2020","links":{"boardgamemechanic":[{"name":"Worker Placement","objectid":"2082"}]}}};
            </script>
            </head><body></body></html>
            """;
    }

    public void Dispose()
    {
        _server.Stop();
        _server.Dispose();
    }
}
