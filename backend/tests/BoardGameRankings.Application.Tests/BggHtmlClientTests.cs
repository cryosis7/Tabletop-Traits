using System.Net;
using System.Text;
using BoardGameRankings.Infrastructure.BggApi;

namespace BoardGameRankings.Application.Tests;

public class BggHtmlClientTests
{
    [Fact]
    public async Task GetUserRatedCollectionAsync_WhenPagerLinksToAnotherPage_FetchesAllPages()
    {
        var responseByPathAndQuery = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["/collection/user/test-user?rated=1&subtype=boardgame&all=1&pageID=1"] = """
            <html><body>
            <div class='geekcollection_pager'>
              <span class='geekpages'>
                Showing 1-3
                Page 1.
                <a href='javascript://' onclick='CE_SetPage( 2 );'>&raquo;</a>
                <a href='javascript://' onclick='CE_SetPage( 2 );'>2</a>
              </span>
            </div>
            <table>
              <tr id='row_1'>
                <td class='collection_objectname'><a href='/boardgame/101/alpha'>Alpha</a></td>
                <td class='collection_rating'><div class='ratingtext'>8</div></td>
              </tr>
              <tr id='row_2'>
                <td class='collection_objectname'><a href='/boardgame/102/bravo'>Bravo</a></td>
                <td class='collection_rating'><div class='ratingtext'>7.5</div></td>
              </tr>
              <tr id='row_3'>
                <td class='collection_objectname'><a href='/boardgame/103/charlie'>Charlie</a></td>
                <td class='collection_rating'><div class='ratingtext'>6</div></td>
              </tr>
            </table>
            </body></html>
            """,
            ["/collection/user/test-user?rated=1&subtype=boardgame&all=1&pageID=2"] = """
            <html><body>
            <div class='geekcollection_pager'>
              <span class='geekpages'>
                Showing 4-5
                <a href='javascript://' onclick='CE_SetPage( 1 );'>1</a>
                Page 2.
              </span>
            </div>
            <table>
              <tr id='row_4'>
                <td class='collection_objectname'><a href='/boardgame/104/delta'>Delta</a></td>
                <td class='collection_rating'><div class='ratingtext'>9</div></td>
              </tr>
              <tr id='row_5'>
                <td class='collection_objectname'><a href='/boardgame/105/echo'>Echo</a></td>
                <td class='collection_rating'><div class='ratingtext'>7</div></td>
              </tr>
            </table>
            </body></html>
            """
        };

        var handler = new StubHttpMessageHandler(responseByPathAndQuery);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://boardgamegeek.com")
        };

        var client = new BggHtmlClient(httpClient);

        var collection = await client.GetUserRatedCollectionAsync("test-user");

        Assert.Equal(5, collection.Count);
        Assert.Equal(
            new[]
            {
                "/collection/user/test-user?rated=1&subtype=boardgame&all=1&pageID=1",
                "/collection/user/test-user?rated=1&subtype=boardgame&all=1&pageID=2"
            },
            handler.RequestedPathAndQueries);
    }

    private sealed class StubHttpMessageHandler(Dictionary<string, string> responseByPathAndQuery) : HttpMessageHandler
    {
        public List<string> RequestedPathAndQueries { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var pathAndQuery = request.RequestUri?.PathAndQuery
                ?? throw new InvalidOperationException("Request URI is required.");

            RequestedPathAndQueries.Add(pathAndQuery);

            if (!responseByPathAndQuery.TryGetValue(pathAndQuery, out var responseBody))
            {
                throw new InvalidOperationException($"Unexpected request: {pathAndQuery}");
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseBody, Encoding.UTF8, "text/html")
            });
        }
    }
}