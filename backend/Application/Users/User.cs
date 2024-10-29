namespace ActivityGameBackend.Application.Users;

public sealed record User
{
    public required string Id { get; init; }
    public required string Email { get; init; }
    public required string Username { get; set; }
    public int Score { get; set; }
    public bool IsHost { get; set; }
}

