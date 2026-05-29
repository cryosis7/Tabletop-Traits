using BoardGameRankings.Domain.Entities;
using BoardGameRankings.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace BoardGameRankings.Infrastructure.Persistence;

public class CachedUserRatingRepository(IBggApiClient bggApiClient, IMemoryCache cache, ILogger<CachedUserRatingRepository> logger) : IUserRatingRepository
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(15);

    public async Task<IReadOnlyList<UserRating>> GetAllAsync(
        string username, CancellationToken cancellationToken = default)
    {
        var key = CacheKey(username);

        if (cache.TryGetValue(key, out IReadOnlyList<UserRating>? cached) && cached is not null)
        {
            logger.LogInformation("UserRating cache hit for {Username}", username);
            return cached;
        }

        logger.LogInformation("UserRating cache miss for {Username}", username);
        var collection = await bggApiClient.GetUserRatedCollectionAsync(username, cancellationToken);

        var ratings = collection
            .DistinctBy(c => c.GameId)
            .Select(c => new UserRating(c.GameId, username, c.Rating))
            .ToList();

        cache.Set(key, (IReadOnlyList<UserRating>)ratings, CacheDuration);

        return ratings;
    }

    public void Invalidate(string username)
    {
        cache.Remove(CacheKey(username));
    }

    private static string CacheKey(string username) => $"user-ratings:{username.ToLowerInvariant()}";
}
