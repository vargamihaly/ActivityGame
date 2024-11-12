using ActivityGameBackend.Application.Exceptions;
using ActivityGameBackend.Application.Games.Validation;
using Microsoft.Extensions.Logging;

namespace ActivityGameBackend.Application.Games;

public interface IGameService : IScoped
{
    Task<Game> GetGameDetailsAsync(Guid id);
    Task<Guid> CreateGameAsync(string userId);
    Task AddPlayerToGameAsync(Guid gameId, string userId);
    Task UpdateGameSettingsAsync(Guid gameId, int timerInMinutes, int maxScore, IEnumerable<MethodType> enabledMethods);
    Task RemovePlayerFromLobbyAsync(Guid gameId, string userId);
    Task<(User nextActivePlayer, Word nextWord)> StartGameAsync(Guid gameId, string currentUserId);
    Task<bool> EndTurnAsync(Guid gameId, string actingPlayerId, string guessingPlayerId);
    Task<Game?> GetActiveGameForPlayerAsync(string userId);
    Task<bool> LeaveGameAsync(Guid gameId, string userId);
}

public class GameService(
    ILogger<GameService> logger,
    IGameValidator gameValidator,
    IUserService userService,
    IGameServiceDataProvider dataProvider) : IGameService
{
    public async Task<Guid> CreateGameAsync(string userId)
    {
        logger.LogInformation("Attempting to create new game for user {UserId}", userId);

        var activeGame = await dataProvider.GetActiveGameForPlayerAsync(userId);
        if (activeGame != null && activeGame.Status == GameStatus.InProgress)
        {
            logger.LogWarning("User {UserId} attempted to create a game while already in an active game {GameId}", userId, activeGame.Id);
            throw new PlayerAlreadyInGameException(userId, activeGame.Id);
        }

        var player = await userService.GetUserByIdAsync(userId) ?? throw new UserNotFoundException(userId);

        var createdGame = await dataProvider.CreateGameAsync(player.Id);
        await dataProvider.UpdateGameAsync(createdGame.Id, GameUpdate.WithHost(player.Id));

        logger.LogInformation("Game {GameId} created successfully for user {UserId}", createdGame.Id, userId);
        return createdGame.Id;
    }

    public async Task<Game> GetGameDetailsAsync(Guid id)
    {
        logger.LogInformation("Fetching game details for game {GameId}", id);
        var game = await dataProvider.GetGameDetailsAsync(id)
            ?? throw new GameNotFoundException(id);
        logger.LogInformation("Game details retrieved successfully for game {GameId}", id);
        return game;
    }

    public async Task AddPlayerToGameAsync(Guid gameId, string userId)
    {
        logger.LogInformation("Adding player {UserId} to game {GameId}", userId, gameId);

        var activeGame = await dataProvider.GetActiveGameForPlayerAsync(userId);
        if (activeGame != null && activeGame.Status == GameStatus.InProgress)
        {
            logger.LogWarning("User {UserId} attempted to join a game while already in an active game {GameId}", userId, activeGame.Id);
            throw new PlayerAlreadyInGameException(userId, activeGame.Id);
        }

        var game = await GetGameDetailsAsync(gameId);

        gameValidator.ValidateGameState(game, GameStatus.Waiting);

        var user = await userService.GetUserByIdAsync(userId)
            ?? throw new UserNotFoundException(userId);

        await dataProvider.AddPlayerToGameAsync(game.Id, user.Id);
        logger.LogInformation("Player {UserId} added successfully to game {GameId}", userId, gameId);
    }

    public async Task RemovePlayerFromLobbyAsync(Guid gameId, string userId)
    {
        logger.LogInformation("Attempting to remove player {UserId} from game {GameId} lobby...", userId, gameId);

        var game = await dataProvider.GetGameDetailsAsync(gameId)
            ?? throw new GameNotFoundException(gameId);

        gameValidator.ValidateGameState(game, GameStatus.Waiting);

        var user = game.Players.FirstOrDefault(p => p.Id == userId);
        if (user is null)
        {
            logger.LogWarning("User {UserId} not found in game {GameId}.", userId, gameId);
            return;
        }

        logger.LogInformation("Removing player {UserId} from game {GameId}...", userId, gameId);
        await dataProvider.RemovePlayerFromGameAsync(game.Id, user.Id);

        if ((bool)user.IsHost)
        {
            logger.LogInformation("User {UserId} is the host of game {GameId}. Reassigning host...", userId, gameId);
            await dataProvider.UpdateGameAsync(game.Id, GameUpdate.WithHost(user.Id));


            var nextHost = game.Players.FirstOrDefault(p => p.Id != user.Id);
            if (nextHost is not null)
            {
                logger.LogInformation("Assigning user {NextHostId} as new host for game {GameId}.", nextHost.Id, gameId);
                await dataProvider.UpdateGameAsync(game.Id, GameUpdate.WithHost(nextHost.Id));
            }
            else
            {
                logger.LogInformation("No other players left in game {GameId}. Removing game.", gameId);
                await dataProvider.RemoveGameAsync(game.Id);
            }
        }

        logger.LogInformation("Player {UserId} successfully removed from game {GameId}.", userId, gameId);
    }

    public async Task UpdateGameSettingsAsync(Guid gameId, int timerInMinutes, int maxScore,
        IEnumerable<MethodType> enabledMethods)
    {
        logger.LogInformation("Updating settings for game {GameId}", gameId);
        var game = await GetGameDetailsAsync(gameId);
        gameValidator.ValidateGameState(game, GameStatus.Waiting);
        gameValidator.ValidateGameSettings(game);

        game.TimerInMinutes = timerInMinutes;
        game.MaxScore = maxScore;
        game.EnabledMethods = enabledMethods.ToList();

        await dataProvider.UpdateGameAsync(game.Id, GameUpdate.WithSettings(timerInMinutes, maxScore, game.EnabledMethods));

        logger.LogInformation("Game settings updated for game {GameId}: Timer={TimerInMinutes}, MaxScore={MaxScore}, EnabledMethods={EnabledMethods}",
            gameId, timerInMinutes, maxScore, string.Join(", ", game.EnabledMethods));
    }

    public async Task<(User nextActivePlayer, Word nextWord)> StartGameAsync(Guid gameId, string currentUserId)
    {
        logger.LogInformation("Starting game {GameId} for user {UserId}", gameId, currentUserId);
        var game = await GetGameDetailsAsync(gameId);
        var player = await userService.GetUserByIdAsync(currentUserId)
            ?? throw new UserNotFoundException(currentUserId);

        gameValidator.ValidateGameState(game, GameStatus.Waiting);
        gameValidator.ValidateGameSettings(game);

        await dataProvider.UpdateGameAsync(game.Id, GameUpdate.WithStatus(GameStatus.InProgress));

        var nextActivePlayer = GetNextActivePlayer(game, player.Id);
        var nextMethodType = GetNextMethodType(game);
        var nextWord = await GetNextWordAsync(game.Id, nextMethodType);

        await dataProvider.CreateRoundAsync(game.Id, nextMethodType, nextWord.Id, nextActivePlayer.Id);

        logger.LogInformation("Game {GameId} started successfully. Next active player: {NextActivePlayer}, Next word: {NextWord}, Next method: {NextMethod}",
            gameId, nextActivePlayer.Username, nextWord.Value, nextMethodType);

        return (nextActivePlayer, nextWord);
    }

    public async Task<bool> EndTurnAsync(Guid gameId, string actingPlayerId, string guessingPlayerId)
    {
        logger.LogInformation("Ending turn for game {GameId}. Acting player: {ActingPlayer}, Guessing player: {GuessingPlayer}",
            gameId, actingPlayerId, guessingPlayerId);

        var game = await GetGameDetailsAsync(gameId);
        _ = await userService.GetUserByIdAsync(actingPlayerId)
            ?? throw new UserNotFoundException(actingPlayerId);

        gameValidator.ValidateGameState(game, GameStatus.InProgress);

        var round = game.Rounds?.LastOrDefault()
            ?? throw new InvalidOperationException("No active round found for the game.");

        await dataProvider.SetRoundWinnerAsync(round.Id, guessingPlayerId);
        await dataProvider.IncrementPlayerScoreAsync(gameId, guessingPlayerId);

        game.Players.First(x => x.Id == guessingPlayerId).Score++;
        
        logger.LogInformation("Round winner set for game {GameId}. Winner: {WinnerId}", gameId, guessingPlayerId);

        if (game.Players.Any(player => player.Score >= game.MaxScore))
        {
            await FinishGameAsync(game, guessingPlayerId);
            return true;
        }

        await StartNextRoundAsync(game, round);
        return false;
    }

    private static MethodType GetNextMethodType(Game game)
    {
        ArgumentNullException.ThrowIfNull(game);

        var lastRound = game.Rounds?.LastOrDefault();
        return lastRound is null
            ? MethodType.Description
            : (MethodType)(((int)lastRound.MethodType + 1) % Enum.GetValues<MethodType>().Length);
    }

    private static User GetNextActivePlayer(Game game, string currentActivePlayerId)
    {
        ArgumentNullException.ThrowIfNull(game);
        ArgumentNullException.ThrowIfNull(game.Players);

        var players = game.Players.ToList();
        var currentIndex = players.FindIndex(p => p.Id == currentActivePlayerId);

        if (currentIndex == -1)
        {
            throw new UserNotFoundException(currentActivePlayerId);
        }

        var nextIndex = (currentIndex + 1) % players.Count;
        return players[nextIndex];
    }

    private async Task<Word> GetNextWordAsync(Guid gameId, MethodType methodType)
    {
        logger.LogInformation("Fetching next word for game {GameId} with method type {MethodType}", gameId, methodType);
        var usedWordIds = await dataProvider.GetUsedWordIdsAsync(gameId);
        var availableWords = await dataProvider.GetAvailableWordsAsync(methodType, usedWordIds);

        if (availableWords.Count == 0)
        {
            logger.LogWarning("No available words for the next round in game {GameId}", gameId);
            throw new InvalidOperationException("No available words for the next round.");
        }

        var nextWord = availableWords[Random.Shared.Next(availableWords.Count)];
        logger.LogInformation("Next word selected for game {GameId}: {Word}", gameId, nextWord.Value);
        return nextWord;
    }

    private async Task FinishGameAsync(Game game, string winnerId)
    {
        await dataProvider.UpdateGameAsync(game.Id, GameUpdate.WithStatus(GameStatus.Finished));
        await dataProvider.UpdateGameAsync(game.Id, GameUpdate.WithWinner(winnerId));

        logger.LogInformation("Game {GameId} finished. Winner: {WinnerId}", game.Id, winnerId);
    }

    private async Task StartNextRoundAsync(Game game, Round currentRound)
    {
        var nextActivePlayer = GetNextActivePlayer(game, currentRound.ActivePlayer.Id);
        var nextMethodType = GetNextMethodType(game);
        var nextWord = await GetNextWordAsync(game.Id, nextMethodType);

        await dataProvider.CreateRoundAsync(game.Id, nextMethodType, nextWord.Id, nextActivePlayer.Id);

        logger.LogInformation("New round created for game {GameId}. Next active player: {NextActivePlayer}, Next word: {NextWord}, Next method: {NextMethod}",
            game.Id, nextActivePlayer.Username, nextWord.Value, nextMethodType);
    }

    public async Task<Game?> GetActiveGameForPlayerAsync(string userId)
    {
        return await dataProvider.GetActiveGameForPlayerAsync(userId);
    }

    public async Task<bool> LeaveGameAsync(Guid gameId, string userId)
    {
        var isGameOver = false;
        
        var game = await dataProvider.GetGameDetailsAsync(gameId) ?? throw new GameNotFoundException(gameId);
        
        if (game.Players.Count() <= 2 && game.Status == GameStatus.InProgress)
        {
            await dataProvider.UpdateGameAsync(gameId, GameUpdate.WithStatus(GameStatus.Finished));
            isGameOver = true;
        }

        await dataProvider.RemovePlayerFromGameAsync(gameId, userId);

        return isGameOver;
    }
}
