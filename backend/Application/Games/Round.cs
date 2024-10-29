namespace ActivityGameBackend.Application.Games;

public sealed record Round
{
    public required Guid Id { get; init; }
    public required Guid GameId { get; set; }
    public string? RoundWinnerId { get; set; }
    public required MethodType MethodType { get; init; }
    public required Word Word { get; init; }
    public required User ActivePlayer { get; init; }
}
