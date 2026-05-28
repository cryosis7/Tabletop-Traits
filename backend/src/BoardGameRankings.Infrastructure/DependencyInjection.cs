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
        var bggBearerToken = configuration["BggApi:BearerToken"] ?? string.Empty;

        // Repositories
        services.AddSingleton<IBoardGameRepository>(new JsonBoardGameRepository(dataPath));
        services.AddSingleton<IUserRatingRepository>(new JsonUserRatingRepository(dataPath));

        // BGG Client (XML API v2)
        services.AddHttpClient<BggXmlApiClient>(client =>
        {
            client.BaseAddress = new Uri(bggBaseUrl);
            client.DefaultRequestHeaders.Add("User-Agent", "BoardGameRankings/1.0");
            if (!string.IsNullOrWhiteSpace(bggBearerToken))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bggBearerToken);
            }
        });

        services.AddScoped<IBggApiClient>(sp =>
            new CachingBggApiClient(
                sp.GetRequiredService<BggXmlApiClient>(),
                sp.GetRequiredService<IBoardGameRepository>()));

        // Application Services
        services.AddScoped<ISyncService, SyncService>();
        services.AddScoped<IMechanismAnalysisService, MechanismAnalysisService>();
        services.AddScoped<ICollectionService, CollectionService>();

        return services;
    }
}
