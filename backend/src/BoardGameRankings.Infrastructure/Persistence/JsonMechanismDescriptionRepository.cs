using System.Reflection;
using System.Text.Json;
using BoardGameRankings.Domain.Entities;
using BoardGameRankings.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace BoardGameRankings.Infrastructure.Persistence;

public class JsonMechanismDescriptionRepository(IMemoryCache cache) : IMechanismDescriptionRepository
{
    private const string CacheKey = "mechanism-descriptions";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromDays(1);

    public async Task<IReadOnlyList<MechanismDescription>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (cache.TryGetValue(CacheKey, out IReadOnlyList<MechanismDescription>? cached) && cached is not null)
            return cached;

        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("BoardGameRankings.Infrastructure.mechanisms.json");

        if (stream is null)
            return [];

        var items = await JsonSerializer.DeserializeAsync<List<MechanismJsonModel>>(
            stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, cancellationToken) ?? [];

        var result = items.Select(m => new MechanismDescription(m.Id, m.Name, m.Description)).ToList().AsReadOnly();

        cache.Set(CacheKey, result, CacheDuration);
        return result;
    }

    private record MechanismJsonModel(int Id, string Name, string Description);
}
