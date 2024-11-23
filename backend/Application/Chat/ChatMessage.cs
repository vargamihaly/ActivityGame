namespace ActivityGameBackend.Application.Chat;

public class ChatMessage
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid GameId { get; init; }
    public string SenderId { get; init; }
    public string Content { get; init; }
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}
