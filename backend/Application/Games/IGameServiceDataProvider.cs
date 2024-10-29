namespace ActivityGameBackend.Application.Games;
public interface IGameServiceDataProvider : IScoped
{
    Task<Game?> GetGameDetailsAsync(Guid id);
    Task<Game> CreateGameAsync(string hostId);
    Task UpdateGameAsync(Guid gameId, GameUpdate update);
    Task AddPlayerToGameAsync(Guid gameId, string playerId);
    Task<Game?> GetActiveGameForPlayerAsync(string userId);
    Task RemovePlayerFromGameAsync(Guid gameId, string userId);
    Task SetRoundWinnerAsync(Guid roundId, string roundWinnerId);
    Task<List<int>> GetUsedWordIdsAsync(Guid gameId);
    Task<List<Word>> GetAvailableWordsAsync(MethodType methodType, List<int> usedWordIds);
    Task CreateRoundAsync(Guid gameId, MethodType methodType, int wordId, string playerId);
    Task IncrementPlayerScoreAsync(Guid gameId, string userId);
    Task ResetPlayersStatus(Guid gameId);
    Task RemoveGameAsync(Guid gameId);
}
