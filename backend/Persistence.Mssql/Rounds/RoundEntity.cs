using System.Diagnostics.CodeAnalysis;
using ActivityGameBackend.Persistence.Mssql.Games;

namespace ActivityGameBackend.Persistence.Mssql.Rounds;
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength")]
public sealed class RoundEntity
{
    public Guid Id { get; init; }
    public required Guid GameId { get; init; }
    public GameEntity? Game { get; init; }
    public string? RoundWinnerId { get; set; }
    public UserEntity? RoundWinner { get; init; }
    public required MethodType MethodType { get; init; }
    public int? WordId { get; init; }
    public WordEntity? Word { get; init; }
    public required string ActivePlayerId { get; init; }
    public UserEntity? ActivePlayer { get; init; }
    public required DateTimeOffset CreatedAtUtc { get; init; }
}
