namespace ActivityGameBackend.Persistence.Mssql.Words;
public sealed class WordEntity
{
    public int Id { get; init; }
    public required string Value { get; init; }
    public required MethodType Method { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; }

}

