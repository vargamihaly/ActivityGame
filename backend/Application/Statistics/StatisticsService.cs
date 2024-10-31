using ActivityGameBackend.Application.Exceptions;
using ActivityGameBackend.Application.Games;
using Microsoft.Extensions.Logging;

namespace ActivityGameBackend.Application.Statistics;

public interface IStatisticsService : IScoped
{
    Task<GlobalStatistics> GetGlobalStatisticsAsync();
    Task<UserStatistics> GetUserStatisticsAsync(string userId);
}

public class StatisticsService(ILogger<StatisticsService> logger, IUserService userService, IStatisticsServiceDataProvider dataProvider) : IStatisticsService
{
    public async Task<GlobalStatistics> GetGlobalStatisticsAsync()
    {
        var finishedGames = await dataProvider.GetFinishedGamesAsync();

        if (finishedGames.Count == 0)
        {
            return new GlobalStatistics
            {
                AverageScore = 0, WinRate = 0, LossRate = 0, PlayerRankings = new List<PlayerRanking>(),
            };
        }

        // Calculate Average Score
        double averageScore = finishedGames.Average(g => g.Players.Average(p => p.Score));

        // Calculate Win and Loss Rates
        var totalGames = finishedGames.Count;
        var totalWins = finishedGames.Count(g => g.Winner?.Id != null);
        var winRate = (double)totalWins / totalGames * 100;
        var lossRate = 100 - winRate;

        // Calculate Player Rankings
        var playerScores = finishedGames
            .SelectMany(g => g.Players)
            .GroupBy(p => p.Id)
            .Select(g => new PlayerRanking
            {
                Username = g.First().Username, TotalScore = g.Sum(p => p.Score),
            })
            .OrderByDescending(p => p.TotalScore)
            .ToList();

        return new GlobalStatistics
        {
            AverageScore = Math.Round(averageScore, 2), WinRate = Math.Round(winRate, 2), LossRate = Math.Round(lossRate, 2), PlayerRankings = playerScores,
        };
    }

    public async Task<UserStatistics> GetUserStatisticsAsync(string userId)
    {
        var user = await userService.GetUserByIdAsync(userId) ?? throw new UserNotFoundException(userId);

        var userGames = await dataProvider.GetGamesForUserAsync(userId);

        var gamesPlayed = userGames.Count;
        var gamesWon = userGames.Count(g => g.Winner!.Id == userId);
        var gamesLost = gamesPlayed - gamesWon;
        var winRate = gamesPlayed > 0 ? (double)gamesWon / gamesPlayed * 100 : 0;
        double averageScore = gamesPlayed > 0 ? userGames.Average(g => g.Players.First(p => p.Id == userId).Score) : 0;

        return new UserStatistics
        {
            Username = user.Username,
            GamesPlayed = gamesPlayed,
            GamesWon = gamesWon,
            GamesLost = gamesLost,
            WinRate = Math.Round(winRate, 2),
            AverageScore = Math.Round(averageScore, 2),
        };
    }
}
