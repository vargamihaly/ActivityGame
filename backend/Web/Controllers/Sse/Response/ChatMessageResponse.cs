namespace ActivityGameBackend.Web.Controllers.Sse.Response;

public record ChatMessageResponse
{
    public Guid Id { get; init; }
    public Guid GameId { get; init; }
    public string SenderId { get; init; }
    public string Content { get; init; }
    public DateTime Timestamp { get; init; }
}
