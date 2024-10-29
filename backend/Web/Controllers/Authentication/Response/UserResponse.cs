namespace ActivityGameBackend.Web.Controllers.Authentication.Response;

public sealed record UserResponse
{
    required public string Id { get; init; }
    required public string Email { get; init; }
    required public string Username { get; init; } = string.Empty;
    required public int? Score { get; init; }
    public bool? IsHost { get; init; }
}
