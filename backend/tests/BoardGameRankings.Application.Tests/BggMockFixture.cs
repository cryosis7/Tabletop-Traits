using BoardGameRankings.DevTools;

namespace BoardGameRankings.Application.Tests;

public class BggMockFixture : IDisposable
{
    public BggMockServer MockServer { get; }
    public string BaseUrl => MockServer.Url;

    public BggMockFixture()
    {
        MockServer = BggMockServer.Start(port: 0); // Random available port
    }

    public void Dispose()
    {
        MockServer.Dispose();
    }
}

[CollectionDefinition("BggMock")]
public class BggMockCollection : ICollectionFixture<BggMockFixture>;
