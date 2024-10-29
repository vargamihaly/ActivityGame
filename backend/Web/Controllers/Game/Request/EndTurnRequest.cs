namespace ActivityGameBackend.Web.Controllers.Game.Request;

public sealed record EndTurnRequest
{
    required public string WinnerUserId { get; init; }
}
