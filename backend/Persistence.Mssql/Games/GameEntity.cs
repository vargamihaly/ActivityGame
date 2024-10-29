using System.Diagnostics.CodeAnalysis;

namespace ActivityGameBackend.Persistence.Mssql.Games;

[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength")]
public sealed class GameEntity
{
    public Guid Id { get; set; }

    public string HostId { get; set; }
    public UserEntity Host { get; set; }

    public string? WinnerId { get; set; }
    public UserEntity? Winner { get; set; }

    public ICollection<GamePlayerEntity> GamePlayers { get; set; } = new List<GamePlayerEntity>();
    public ICollection<RoundEntity> Rounds { get; set; } = new List<RoundEntity>();

    public GameStatus Status { get; set; }
    public int TimerInMinutes { get; set; }
    public int MaxScore { get; set; }
    public List<MethodType> EnabledMethods { get; set; } = new();
    public DateTimeOffset CreatedAtUtc { get; init; }

    public override string? ToString()
    {
        return $"GameEntity: Id={Id}, Host={Host}, Timer={TimerInMinutes}, MaxScore={MaxScore}, Status={Status}";
    }
}
