using BoardGameRankings.Application.Interfaces;
using BoardGameRankings.Application.Services;
using BoardGameRankings.Domain.Interfaces;
using BoardGameRankings.Infrastructure.BggApi;
using BoardGameRankings.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BoardGameRankings.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string dataPath, IConfiguration configuration)
    {
        var bggBaseUrl = configuration["BggApi:BaseUrl"] ?? "https://boardgamegeek.com";

        // Repositories
        services.AddSingleton<IBoardGameRepository>(new JsonBoardGameRepository(dataPath));
        services.AddSingleton<IUserRatingRepository>(new JsonUserRatingRepository(dataPath));

        // BGG Client (HTML scraper)
        services.AddHttpClient<BggHtmlClient>(client =>
        {
            client.BaseAddress = new Uri(bggBaseUrl);
            client.DefaultRequestHeaders.Add("User-Agent", "BoardGameRankings/1.0");
        });

        services.AddScoped<IBggApiClient>(sp =>
            new CachingBggApiClient(
                sp.GetRequiredService<BggHtmlClient>(),
                sp.GetRequiredService<IBoardGameRepository>()));

        // Application Services
        services.AddScoped<ISyncService, SyncService>();
        services.AddScoped<IMechanismAnalysisService, MechanismAnalysisService>();
        services.AddScoped<ICollectionService, CollectionService>();

        return services;
    }
}
