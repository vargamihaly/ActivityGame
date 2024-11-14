using System.Text.Json.Serialization;

namespace ActivityGameBackend.Application.Games;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GameStatus
{
    Waiting = 0,
    InProgress = 1,
    Finished = 2,
    TimeUp = 3,
}
