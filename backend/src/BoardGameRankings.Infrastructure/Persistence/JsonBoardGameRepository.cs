using System.Text.Json;
using BoardGameRankings.Domain.Entities;
using BoardGameRankings.Domain.Interfaces;

namespace BoardGameRankings.Infrastructure.Persistence;

public class JsonBoardGameRepository : IBoardGameRepository
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private Dictionary<int, BoardGame>? _cache;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public JsonBoardGameRepository(string basePath)
    {
        _filePath = Path.Combine(basePath, "games.json");
    }

    public async Task<IReadOnlyList<BoardGame>> GetByIdsAsync(IEnumerable<int> gameIds)
    {
        var cache = await GetCacheAsync();
        return gameIds
            .Where(cache.ContainsKey)
            .Select(id => cache[id])
            .ToList();
    }

    public async Task SaveAsync(IReadOnlyList<BoardGame> games)
    {
        await _lock.WaitAsync();
        try
        {
            var cache = await LoadFromDiscAsync();

            foreach (var game in games)
            {
                cache[game.Id] = game;
            }

            _cache = cache;

            var directory = Path.GetDirectoryName(_filePath)!;
            Directory.CreateDirectory(directory);

            var records = cache.Values.Select(g => new BoardGameRecord
            {
                Id = g.Id,
                Name = g.Name,
                YearPublished = g.YearPublished,
                ThumbnailUrl = g.ThumbnailUrl,
                Mechanisms = g.Mechanisms.Select(m => new MechanismRecord { Id = m.Id, Name = m.Name }).ToList()
            }).ToList();

            var json = JsonSerializer.Serialize(records, JsonOptions);
            await File.WriteAllTextAsync(_filePath, json);
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<Dictionary<int, BoardGame>> GetCacheAsync()
    {
        if (_cache != null)
            return _cache;

        await _lock.WaitAsync();
        try
        {
            _cache ??= await LoadFromDiscAsync();
            return _cache;
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<Dictionary<int, BoardGame>> LoadFromDiscAsync()
    {
        if (!File.Exists(_filePath))
            return new Dictionary<int, BoardGame>();

        var json = await File.ReadAllTextAsync(_filePath);
        var records = JsonSerializer.Deserialize<List<BoardGameRecord>>(json, JsonOptions);
        if (records == null)
            return new Dictionary<int, BoardGame>();

        return records.ToDictionary(
            r => r.Id,
            r => new BoardGame(
                r.Id,
                r.Name,
                r.YearPublished,
                r.ThumbnailUrl,
                r.Mechanisms.Select(m => new Mechanism(m.Id, m.Name)).ToList()
            )
        );
    }

    private record BoardGameRecord
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public int? YearPublished { get; init; }
        public string? ThumbnailUrl { get; init; }
        public List<MechanismRecord> Mechanisms { get; init; } = new();
    }

    private record MechanismRecord
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }
}
