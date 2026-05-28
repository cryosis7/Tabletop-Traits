using System.Text.Json;
using BoardGameRankings.Domain.Entities;
using BoardGameRankings.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace BoardGameRankings.Infrastructure.Persistence;

public class JsonMechanismDescriptionRepository(IConfiguration configuration, IMemoryCache cache) : IMechanismDescriptionRepository
{
    private const string CacheKey = "mechanism-descriptions";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromDays(1);

    public async Task<IReadOnlyList<MechanismDescription>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (cache.TryGetValue(CacheKey, out IReadOnlyList<MechanismDescription>? cached) && cached is not null)
            return cached;

        var defaultDataPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "data");
        var configuredPath = configuration["DataPath"];
        var filePath = configuredPath is not null
            ? Path.Combine(configuredPath, "mechanisms.json")
            : Path.Combine(defaultDataPath, "mechanisms.json");

        // Mechanisms are static reference data; fall back to the project data path if not in the configured path
        if (!File.Exists(filePath))
            filePath = Path.Combine(defaultDataPath, "mechanisms.json");

        if (!File.Exists(filePath))
            return [];

        var json = await File.ReadAllTextAsync(filePath, cancellationToken);
        var items = JsonSerializer.Deserialize<List<MechanismJsonModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];

        var result = items.Select(m => new MechanismDescription(m.Id, m.Name, m.Description)).ToList().AsReadOnly();

        cache.Set(CacheKey, result, CacheDuration);
        return result;
    }

    private record MechanismJsonModel(int Id, string Name, string Description);
}
