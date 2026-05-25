using BoardGameRankings.Application.DTOs;

namespace BoardGameRankings.Application.Interfaces;

public interface ISyncService
{
    Task<SyncStatusDto> SyncUserCollectionAsync(string username, CancellationToken cancellationToken = default);
}
