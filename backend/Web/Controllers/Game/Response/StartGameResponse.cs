namespace ActivityGameBackend.Web.Controllers.Game.Response
{
    public sealed record StartGameResponse
    {
        required public Guid GameId { get; set; }
        required public string NextActivePlayer { get; init; } = string.Empty;
        required public string NextWord { get; init; } = string.Empty;
        required public string MethodType { get; init; } = string.Empty;
    }
}
