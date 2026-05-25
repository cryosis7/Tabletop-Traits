namespace BoardGameRankings.Domain.Entities;

public class Mechanism
{
    public int Id { get; }
    public string Name { get; }

    public Mechanism(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public override bool Equals(object? obj) =>
        obj is Mechanism other && Id == other.Id;

    public override int GetHashCode() => Id.GetHashCode();
}
