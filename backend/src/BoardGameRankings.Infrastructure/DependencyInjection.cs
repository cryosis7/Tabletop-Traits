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
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var bggBaseUrl = configuration["BggApi:BaseUrl"] ?? "https://boardgamegeek.com";
        var bggBearerToken = configuration["BggApi:BearerToken"] ?? string.Empty;

        // In-memory cache (singleton by default)
        services.AddMemoryCache();

        // BGG API Client
        services.AddHttpClient<IBggApiClient, BggXmlApiClient>(client =>
        {
            client.BaseAddress = new Uri(bggBaseUrl);
            client.DefaultRequestHeaders.Add("User-Agent", "BoardGameRankings/1.0");
            if (!string.IsNullOrWhiteSpace(bggBearerToken))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bggBearerToken);
            }
        });

        // Repositories (singleton - backed by IMemoryCache)
        services.AddSingleton<IBoardGameRepository, CachedBoardGameRepository>();
        services.AddSingleton<IUserRatingRepository, CachedUserRatingRepository>();
        services.AddSingleton<IMechanismDescriptionRepository, JsonMechanismDescriptionRepository>();

        // Application Services
        services.AddScoped<ISyncService, SyncService>();
        services.AddScoped<IMechanismAnalysisService, MechanismAnalysisService>();
        services.AddScoped<ICollectionService, CollectionService>();
        services.AddScoped<IMechanismDescriptionService, MechanismDescriptionService>();

        return services;
    }
}
