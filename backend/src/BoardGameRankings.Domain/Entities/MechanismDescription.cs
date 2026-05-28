namespace BoardGameRankings.Domain.Entities;

public class MechanismDescription(int id, string name, string description)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
    public string Description { get; } = description;
}
