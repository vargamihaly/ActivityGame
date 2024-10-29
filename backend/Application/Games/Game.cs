namespace ActivityGameBackend.Application.Games;
public sealed record Game
{
    public required Guid Id { get; init; }
    public required User Host { get; init; }
    public required int TimerInMinutes { get; set; }
    public required int MaxScore { get; set; }
    public required IEnumerable<User> Players { get; init; } = [];
    public required IEnumerable<Round>? Rounds { get; set; } = [];
    public required GameStatus Status { get; init; }
    public required IEnumerable<MethodType> EnabledMethods { get; set; } = new List<MethodType>();
    public User? Winner { get; set; }
}
