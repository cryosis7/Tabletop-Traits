using BoardGameRankings.Application.DTOs;
using BoardGameRankings.Application.Interfaces;
using BoardGameRankings.Domain.Interfaces;

namespace BoardGameRankings.Application.Services;

public class MechanismDescriptionService(IMechanismDescriptionRepository repository) : IMechanismDescriptionService
{
    public async Task<IReadOnlyList<MechanismDescriptionDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var mechanisms = await repository.GetAllAsync(cancellationToken);
        return mechanisms.Select(m => new MechanismDescriptionDto(m.Id, m.Name, m.Description)).ToList();
    }
}
