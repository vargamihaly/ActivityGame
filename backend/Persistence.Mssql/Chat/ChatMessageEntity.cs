using ActivityGameBackend.Persistence.Mssql.Games;

namespace ActivityGameBackend.Persistence.Mssql.Chat;

public sealed class ChatMessageEntity
{
    public Guid Id { get; init; }
    public Guid GameId { get; init; }
    public GameEntity Game { get; init; }
    public string SenderId { get; init; }
    public UserEntity Sender { get; init; }
    public string Content { get; init; }
    public DateTimeOffset Timestamp { get; init; }
}
