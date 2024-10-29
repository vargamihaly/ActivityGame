using ActivityGameBackend.Application.Exceptions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ActivityGameBackend.Persistence.Mssql.Games;

public class GameServiceDataProvider(ApplicationDbContext context, IMapper mapper) : IGameServiceDataProvider
{

    public async Task<Game?> GetGameDetailsAsync(Guid id)
    {
        var gameEntity = await context.Games
            .Include(g => g.Host)
            .Include(g => g.GamePlayers)
                .ThenInclude(gp => gp.User)
            .Include(g => g.Rounds)
                .ThenInclude(r => r.Word)
            .FirstOrDefaultAsync(g => g.Id == id)
            .ConfigureAwait(false);

        var game = mapper.Map<Game>(gameEntity);

        return game;
    }

    public async Task<Game> CreateGameAsync(string hostId)
    {
        var gameEntity = new GameEntity
        {
            Id = Guid.NewGuid(),
            HostId = hostId,
            Status = GameStatus.Waiting,
            TimerInMinutes = 2,
            MaxScore = 10,
            EnabledMethods = new List<MethodType> { MethodType.Description },
        };

        var gamePlayer = new GamePlayerEntity
        {
            GameId = gameEntity.Id,
            UserId = hostId,
            IsHost = true,
            Score = 0,
        };

        context.Games.Add(gameEntity);
        context.GamePlayers.Add(gamePlayer);

        await context.SaveChangesAsync().ConfigureAwait(false);

        var game = mapper.Map<Game>(gameEntity);

        return game;
    }

    public async Task UpdateGameAsync(Guid gameId, GameUpdate update)
    {
        var gameEntity = await context.Games
            .FirstAsync(g => g.Id == gameId)
            .ConfigureAwait(false);

        if (update.Status.HasValue)
            gameEntity.Status = update.Status.Value;

        if (update.HostId != null)
            gameEntity.HostId = update.HostId;

        if (update.WinnerId != null)
            gameEntity.WinnerId = update.WinnerId;

        if (update.TimerInMinutes.HasValue)
            gameEntity.TimerInMinutes = update.TimerInMinutes.Value;

        if (update.MaxScore.HasValue)
            gameEntity.MaxScore = update.MaxScore.Value;

        if (update.EnabledMethods != null)
            gameEntity.EnabledMethods = update.EnabledMethods.ToList();

        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task RemoveGameAsync(Guid gameId)
    {
        var gameEntity = await context.Games.FirstAsync(g => g.Id == gameId).ConfigureAwait(false);
        context.Games.Remove(gameEntity);
        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task UpdateUserHostStatusAsync(Guid gameId, string userId)
    {
        var gamePlayer = await context.GamePlayers
            .FirstOrDefaultAsync(gp => gp.GameId == gameId && gp.UserId == userId)
            .ConfigureAwait(false);

        if (gamePlayer != null)
        {
            gamePlayer.IsHost = true;
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }


    public async Task<Game?> GetActiveGameForPlayerAsync(string userId)
    {
        var gameEntity = await context.Games
          .Include(g => g.GamePlayers)
          .ThenInclude(gp => gp.User)
          .Include(g => g.Host)
          .Include(g => g.Rounds)
          .FirstOrDefaultAsync(g => g.GamePlayers.Any(p => p.UserId == userId) && g.Status != GameStatus.Finished);

        var game = mapper.Map<Game>(gameEntity);

        return game;
    }


    public async Task AddPlayerToGameAsync(Guid gameId, string playerId)
    {
        var gamePlayerExists = await context.GamePlayers
            .AnyAsync(gp => gp.GameId == gameId && gp.UserId == playerId)
            .ConfigureAwait(false);

        if (!gamePlayerExists)
        {
            var gamePlayer = new GamePlayerEntity
            {
                GameId = gameId,
                UserId = playerId,
                IsHost = false,
                Score = 0,
            };

            context.GamePlayers.Add(gamePlayer);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }

    public async Task RemovePlayerFromGameAsync(Guid gameId, string userId)
    {
        var gamePlayer = await context.GamePlayers
            .FirstOrDefaultAsync(gp => gp.GameId == gameId && gp.UserId == userId)
            .ConfigureAwait(false);

        if (gamePlayer != null)
        {
            context.GamePlayers.Remove(gamePlayer);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }

    public async Task SetRoundWinnerAsync(Guid roundId, string roundWinnerId)
    {
        var currentRoundEntity = await context.Rounds.FirstAsync(r => r.Id == roundId).ConfigureAwait(false);
        currentRoundEntity.RoundWinnerId = roundWinnerId;

        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task UpdatePlayerScoreAsync(Guid gameId, string userId, int scoreIncrement)
    {
        var gamePlayer = await context.GamePlayers
            .FirstOrDefaultAsync(gp => gp.GameId == gameId && gp.UserId == userId)
            .ConfigureAwait(false);

        if (gamePlayer is null)
            throw new AppException("Player not found in game", ErrorCode.UserNotFound);

        gamePlayer.Score += scoreIncrement;

        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<List<int>> GetUsedWordIdsAsync(Guid gameId)
    {
        return await context.Rounds
            .Where(r => r.GameId == gameId && r.WordId.HasValue)
            .Select(r => r.WordId!.Value)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task<List<Word>> GetAvailableWordsAsync(MethodType methodType, List<int> usedWordIds)
    {
        var wordEntities = await context.Words
            .Where(w => w.Method == methodType && !usedWordIds.Contains(w.Id))
            .Select(w => w)
            .ToListAsync()
            .ConfigureAwait(false);

        var words = mapper.Map<List<Word>>(wordEntities);

        return words;
    }

    public async Task CreateRoundAsync(Guid gameId, MethodType methodType, int wordId, string playerId)
    {
        var roundEntity = new RoundEntity
        {
            GameId = gameId,
            MethodType = methodType,
            WordId = wordId,
            ActivePlayerId = playerId,
            CreatedAtUtc = DateTime.UtcNow,
        };

        context.Rounds.Add(roundEntity);
        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task IncrementPlayerScoreAsync(Guid gameId, string userId)
    {
        var gamePlayer = await context.GamePlayers
            .FirstOrDefaultAsync(gp => gp.GameId == gameId && gp.UserId == userId)
            .ConfigureAwait(false);

        if (gamePlayer is null)
            throw new InvalidOperationException($"Player with ID {userId} not found in game {gameId}.");

        gamePlayer.Score += 1;
        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task ResetPlayersStatus(Guid gameId)
    {
        var gamePlayers = await context.GamePlayers
            .Where(gp => gp.GameId == gameId)
            .ToListAsync()
            .ConfigureAwait(false);

        foreach (var gamePlayer in gamePlayers)
        {
            gamePlayer.IsHost = false;
            gamePlayer.Score = 0;
        }

        await context.SaveChangesAsync().ConfigureAwait(false);
    }


}
