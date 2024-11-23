namespace ActivityGameBackend.Web.Controllers.Sse.Request;

public record SendMessageRequest
{
    public required string Message { get; init; }
}
