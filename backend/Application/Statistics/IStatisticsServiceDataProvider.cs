using ActivityGameBackend.Application.Games;

namespace ActivityGameBackend.Application.Statistics;

public interface IStatisticsServiceDataProvider : IScoped
{
    Task<List<Game>> GetFinishedGamesAsync();
    Task<List<Game>> GetGamesForUserAsync(string userId);
}
