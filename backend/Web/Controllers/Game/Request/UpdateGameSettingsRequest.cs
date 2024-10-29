using ActivityGameBackend.Application.Games;

namespace ActivityGameBackend.Web.Controllers.Game.Request;

public sealed record UpdateGameSettingsRequest
{
    required public int Timer { get; set; }
    required public int MaxScore { get; set; }
    required public List<MethodType> EnabledMethods { get; init; }
}
