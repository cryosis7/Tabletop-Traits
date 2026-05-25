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

    public void Dispose()
    {
        _server.Stop();
        _server.Dispose();
    }
}
