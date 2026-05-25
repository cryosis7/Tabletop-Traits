using System.Text.Json;
using BoardGameRankings.Domain.Entities;
using BoardGameRankings.Domain.Interfaces;

namespace BoardGameRankings.Infrastructure.Persistence;

public class JsonBoardGameRepository : IBoardGameRepository
{
    private readonly string _basePath;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public JsonBoardGameRepository(string basePath)
    {
        _basePath = basePath;
    }

    public async Task<IReadOnlyList<BoardGame>> GetAllAsync(string username)
    {
        var filePath = GetFilePath(username);
        if (!File.Exists(filePath))
            return Array.Empty<BoardGame>();

        var json = await File.ReadAllTextAsync(filePath);
        var records = JsonSerializer.Deserialize<List<BoardGameRecord>>(json, JsonOptions);
        if (records == null)
            return Array.Empty<BoardGame>();

        return records.Select(r => new BoardGame(
            r.Id,
            r.Name,
            r.YearPublished,
            r.ThumbnailUrl,
            r.Mechanisms.Select(m => new Mechanism(m.Id, m.Name)).ToList()
        )).ToList();
    }

    public async Task<BoardGame?> GetByIdAsync(string username, int gameId)
    {
        var games = await GetAllAsync(username);
        return games.FirstOrDefault(g => g.Id == gameId);
    }

    public async Task SaveAsync(string username, IReadOnlyList<BoardGame> games)
    {
        var filePath = GetFilePath(username);
        var directory = Path.GetDirectoryName(filePath)!;
        Directory.CreateDirectory(directory);

        var records = games.Select(g => new BoardGameRecord
        {
            Id = g.Id,
            Name = g.Name,
            YearPublished = g.YearPublished,
            ThumbnailUrl = g.ThumbnailUrl,
            Mechanisms = g.Mechanisms.Select(m => new MechanismRecord { Id = m.Id, Name = m.Name }).ToList()
        }).ToList();

        var json = JsonSerializer.Serialize(records, JsonOptions);
        await File.WriteAllTextAsync(filePath, json);
    }

    private string GetFilePath(string username) =>
        Path.Combine(_basePath, username.ToLowerInvariant(), "games.json");

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
