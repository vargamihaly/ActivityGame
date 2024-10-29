using ActivityGameBackend.Application.Exceptions;

namespace ActivityGameBackend.Application.Games.Validation;

public interface IGameValidator : IScoped
{
    void ValidateGameState(Game game, GameStatus expectedStatus);
    void ValidateGameSettings(Game game);
}

public class GameValidator : IGameValidator
{
    public void ValidateGameState(Game game, GameStatus expectedStatus)
    {
        ArgumentNullException.ThrowIfNull(game);
        if (game.Status != expectedStatus)
        {
            throw new InvalidGameStateException(expectedStatus);
        }
    }

    public void ValidateGameSettings(Game game)
    {
        ArgumentNullException.ThrowIfNull(game);
        if (game.TimerInMinutes <= 1)
        {
            throw new InvalidGameSettingsException("timer", 1);
        }
        if (game.MaxScore <= 1)
        {
            throw new InvalidGameSettingsException("max score", 1);
        }
        if (game.Players == null || !game.Players.Any())
        {
            throw new InvalidPlayerCountException();
        }
    }
}
