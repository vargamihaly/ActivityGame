namespace ActivityGameBackend.Persistence.Mssql.Games;

public class GamePlayerEntity
{
    public Guid GameId { get; set; }
    public GameEntity Game { get; set; }

    public string UserId { get; set; }
    public UserEntity User { get; set; }

    public int Score { get; set; }
    public bool IsHost { get; set; }
}
