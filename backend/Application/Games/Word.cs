namespace ActivityGameBackend.Application.Games;

public sealed record Word
{
    public required int Id { get; init; }
    public required string Value { get; init; }
    public required MethodType Method { get; init; }
}
