using BoardGameRankings.Domain.Entities;

namespace BoardGameRankings.Domain.Interfaces;

public interface IMechanismDescriptionRepository
{
    Task<IReadOnlyList<MechanismDescription>> GetAllAsync(CancellationToken cancellationToken = default);
}
