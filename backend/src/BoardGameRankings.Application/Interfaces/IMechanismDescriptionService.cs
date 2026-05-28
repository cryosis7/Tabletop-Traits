using BoardGameRankings.Application.DTOs;

namespace BoardGameRankings.Application.Interfaces;

public interface IMechanismDescriptionService
{
    Task<IReadOnlyList<MechanismDescriptionDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
