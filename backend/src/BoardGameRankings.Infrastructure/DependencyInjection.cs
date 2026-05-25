using BoardGameRankings.Application.Interfaces;
using BoardGameRankings.Application.Services;
using BoardGameRankings.Domain.Interfaces;
using BoardGameRankings.Infrastructure.BggApi;
using BoardGameRankings.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace BoardGameRankings.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string dataPath)
    {
        // Repositories
        services.AddSingleton<IBoardGameRepository>(new JsonBoardGameRepository(dataPath));
        services.AddSingleton<IUserRatingRepository>(new JsonUserRatingRepository(dataPath));

        // BGG Clients
        services.AddHttpClient<BggXmlApiClient>(client =>
        {
            client.DefaultRequestHeaders.Add("User-Agent", "BoardGameRankings/1.0");
        });

        services.AddHttpClient<BggHtmlClient>(client =>
        {
            client.DefaultRequestHeaders.Add("User-Agent", "BoardGameRankings/1.0");
        });

        services.AddScoped<IBggApiClient, BggFallbackApiClient>();

        // Application Services
        services.AddScoped<ISyncService, SyncService>();
        services.AddScoped<IMechanismAnalysisService, MechanismAnalysisService>();
        services.AddScoped<ICollectionService, CollectionService>();

        return services;
    }
}
