namespace BoardGameRankings.Domain.Entities;

public class BoardGame
{
    public int Id { get; }
    public string Name { get; }
    public int? YearPublished { get; }
    public string? ThumbnailUrl { get; }
    public IReadOnlyList<Mechanism> Mechanisms { get; }

    public BoardGame(int id, string name, int? yearPublished, string? thumbnailUrl, IReadOnlyList<Mechanism> mechanisms)
    {
        Id = id;
        Name = name;
        YearPublished = yearPublished;
        ThumbnailUrl = thumbnailUrl;
        Mechanisms = mechanisms;
    }
}
