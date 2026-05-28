using BoardGameRankings.Domain.Entities;
using BoardGameRankings.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace BoardGameRankings.Infrastructure.Persistence;

public class CachedUserRatingRepository(IBggApiClient bggApiClient, IMemoryCache cache) : IUserRatingRepository
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(1);

    public async Task<IReadOnlyList<UserRating>> GetAllAsync(
        string username, CancellationToken cancellationToken = default)
    {
        var key = CacheKey(username);

        if (cache.TryGetValue(key, out IReadOnlyList<UserRating>? cached) && cached is not null)
            return cached;

        var collection = await bggApiClient.GetUserRatedCollectionAsync(username, cancellationToken);

        var ratings = collection
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
