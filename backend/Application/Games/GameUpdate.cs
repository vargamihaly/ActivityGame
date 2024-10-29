namespace ActivityGameBackend.Application.Games;

public class GameUpdate
{
    public GameStatus? Status { get; set; }
    public string? HostId { get; set; }
    public string? WinnerId { get; set; }
    public int? TimerInMinutes { get; set; }
    public int? MaxScore { get; set; }
    public IEnumerable<MethodType>? EnabledMethods { get; set; }
    
    // Helper factory methods for common update scenarios
    public static GameUpdate WithStatus(GameStatus status) => new() { Status = status };
    public static GameUpdate WithHost(string hostId) => new() { HostId = hostId };
    public static GameUpdate WithWinner(string winnerId) => new() { WinnerId = winnerId };
    public static GameUpdate WithSettings(int timer, int maxScore, IEnumerable<MethodType> methods) => new()
    {
        TimerInMinutes = timer,
        MaxScore = maxScore,
        EnabledMethods = methods,
    };
}
