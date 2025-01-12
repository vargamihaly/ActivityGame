using ActivityGameBackend.Application.Exceptions;

namespace ActivityGameBackend.Application.Games.Validation;

public interface IGameValidator : IScoped
{
    void ValidateGameState(Game game, GameStatus expectedStatus);
    void ValidateGameSettings(int timerInMinutes, int maxScore, IEnumerable<User> players);
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

    public void ValidateGameSettings(int timerInMinutes, int maxScore, IEnumerable<User> players)
    {
        Console.WriteLine("Validating game settings");
        Console.WriteLine($"Timer: {timerInMinutes}");
        Console.WriteLine($"Max score: {maxScore}");    
        if (timerInMinutes < 1)
        {
            throw new InvalidGameSettingsException("timer", 1);
        }
        if (maxScore <= 1)
        {
            throw new InvalidGameSettingsException("max score", 1);
        }
        if (players == null || !players.Any())
        {
            throw new InvalidPlayerCountException();
        }
    }
}
