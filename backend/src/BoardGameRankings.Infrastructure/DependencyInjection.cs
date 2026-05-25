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

        // BGG API Client
        services.AddHttpClient<IBggApiClient, BggApiClient>(client =>
        {
            client.DefaultRequestHeaders.Add("User-Agent", "BoardGameRankings/1.0");
        });

        // Application Services
        services.AddScoped<ISyncService, SyncService>();
        services.AddScoped<IMechanismAnalysisService, MechanismAnalysisService>();
        services.AddScoped<ICollectionService, CollectionService>();

        return services;
    }
}
