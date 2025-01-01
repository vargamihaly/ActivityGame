using ActivityGameBackend.Application.Games;

namespace ActivityGameBackend.Web.Controllers.Game.Response;

public sealed record GameResponse
{
    required public Guid Id { get; init; }
    required public string HostId { get; init; }
    required public int TimerInMinutes { get; init; }
    required public int MaxScore { get; init; }
    required public List<PlayerResponse> Players { get; init; } = [];
    required public GameStatus Status { get; init; }
    public RoundResponse? CurrentRound { get; init; }
    required public List<MethodType> EnabledMethods { get; set; }
}

public sealed record PlayerResponse
{
    required public string Id { get; init; }
    required public string Username { get; init; } = string.Empty;
    required public int? Score { get; init; }
    public bool? IsHost { get; init; }
}

public sealed record RoundResponse
{
    public int RoundNumber { get; set; }
    required public MethodType MethodType { get; init; }
    required public string Word { get; init; } = string.Empty;
    required public string ActivePlayerUsername { get; init; }
    required public DateTimeOffset CreatedAtUtc { get; init; }
}
