namespace ActivityGameBackend.Web.Controllers.Game.Response
{
    public sealed record EndTurnResponse
    {
        required public bool IsGameWon { get; init; }
    }
}
