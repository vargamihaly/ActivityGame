using System.Net;
using System.Text.Json.Serialization;
using ActivityGameBackend.Application.Games;

namespace ActivityGameBackend.Application.Exceptions;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ErrorCode
{
    GameNotFound = 1001,

    PlayerAlreadyInGame = 1002,

    InvalidGameState = 1003,

    HostValidationFailed = 1004,

    InvalidGameSettings = 1005,

    InvalidPlayerCount = 1006,

    UserNotFound = 1007,
    UserNotAuthenticated = 1008,

}

public class AppException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public ErrorCode ErrorCode { get; }

    public AppException(string message, ErrorCode errorCode, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }
}

public class GameNotFoundException(Guid gameId) : AppException($"Game with ID {gameId} not found", ErrorCode.GameNotFound, HttpStatusCode.NotAcceptable);
public class PlayerAlreadyInGameException(string userId,
                                          Guid gameId) : AppException($"Player {userId} is already in an active game {gameId}", ErrorCode.PlayerAlreadyInGame, HttpStatusCode.Conflict);
public class InvalidGameStateException(GameStatus expectedStatus) : AppException($"Invalid game state. Expected: {expectedStatus}", ErrorCode.InvalidGameState, HttpStatusCode.BadRequest);

public class InvalidGameSettingsException(string setting, int minValue) : AppException($"Invalid game settings. {setting} must be greater than {minValue}", ErrorCode.InvalidGameSettings, HttpStatusCode.UnprocessableEntity);

public class InvalidPlayerCountException() : AppException("The game must have at least two player.", ErrorCode.InvalidPlayerCount);

public class UserNotFoundException(string userId) : AppException($"User with ID {userId} not found", ErrorCode.UserNotFound, HttpStatusCode.NotFound);
