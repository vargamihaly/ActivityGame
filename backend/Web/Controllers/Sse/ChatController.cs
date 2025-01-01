using ActivityGameBackend.Application.Chat;
using ActivityGameBackend.Web.Controllers.Sse.Request;
using ActivityGameBackend.Web.Controllers.Sse.Response;
using ActivityGameBackend.Web.Services;
using ActivityGameBackend.Web.Shared;
using AutoMapper;

namespace ActivityGameBackend.Web.Controllers.Sse;

[ApiController]
[Route("api/games/{gameId}/chat")]
public class ChatController(IMapper mapper, IChatService chatService, IGameEventService gameEventService, IIdentityService identityService) : ControllerBase
{
    private string CurrentUserId => identityService.GetId();

    [HttpPost("messages")]
    [ProducesResponseType(typeof(ApiResponse<ChatMessageResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendMessage([FromRoute] Guid gameId, [FromBody] SendMessageRequest request)
    {
        var savedMessage = await chatService.SaveMessageAsync(gameId, CurrentUserId, request.Message);

        await gameEventService.BroadcastEventAsync(
            gameId,
            "ChatMessage",
            new
            {
                messageId = savedMessage.Id,
                userId = savedMessage.SenderId,
                content = savedMessage.Content,
                timestamp = savedMessage.Timestamp,
            }
        );

        return Ok(new ApiResponse<ChatMessageResponse>
        {
            Success = true,
            Data = mapper.Map<ChatMessageResponse>(savedMessage),
        });
    }

    [HttpGet("messages")]
    [ProducesResponseType(typeof(ApiResponse<List<ChatMessageResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetChatHistory([FromRoute] Guid gameId)
    {
        var history = await chatService.GetGameChatHistoryAsync(gameId);
        return Ok(new ApiResponse<List<ChatMessageResponse>>
        {
            Success = true,
            Data = mapper.Map<List<ChatMessageResponse>>(history),
        });
    }
}
