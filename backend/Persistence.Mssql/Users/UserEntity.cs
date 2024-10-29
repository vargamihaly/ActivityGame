using ActivityGameBackend.Persistence.Mssql.Games;

namespace ActivityGameBackend.Persistence.Mssql.Users;
public sealed class UserEntity
{
    public string Id { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Username { get; set; } = default!;

    public ICollection<GamePlayerEntity> GamePlayers { get; set; } = new List<GamePlayerEntity>();

    public DateTimeOffset CreatedAtUtc { get; init; }
}
