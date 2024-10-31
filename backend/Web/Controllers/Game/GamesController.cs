using System.Net.Mime;
using ActivityGameBackend.Application.Games;
using ActivityGameBackend.Application.Users;
using ActivityGameBackend.Web.Controllers.Game.Request;
using ActivityGameBackend.Web.Controllers.Game.Response;
using ActivityGameBackend.Web.Services;
using ActivityGameBackend.Web.Shared;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace ActivityGameBackend.Web.Controllers.Game;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GamesController(
    IMapper mapper,
    ILogger<GamesController> logger,
    IIdentityService identityService,
    IGameService gameService,
    IUserService userService,
    IGameEventService gameEventService)
    : ControllerBase
{
    private string CurrentUserId => identityService.GetId();

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentGame()
    {
        var userId = identityService.GetId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new ApiResponse
            {
                Success = false,
                Message = "User not authenticated",
            });
        }

        var game = await gameService.GetActiveGameForPlayerAsync(userId);
        if (game == null)
        {
            return Ok(new ApiResponse<GameResponse>
            {
                Success = true,
                Data = null,
            });
        }

        var gameResponse = mapper.Map<GameResponse>(game);
        return Ok(new ApiResponse<GameResponse>
        {
            Success = true,
            Data = gameResponse,
        });
    }

    [HttpGet("details/{gameId:guid}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse<GameResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGameDetails(Guid gameId)
    {
        var game = await gameService.GetGameDetailsAsync(gameId);

        var gameResponse = mapper.Map<GameResponse>(game);
        return Ok(new ApiResponse<GameResponse>
        {
            Success = true,
            Data = gameResponse,
        });
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<CreateGameResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateGame()
    {
        logger.LogInformation("Create Game endpoint hit for user {UserId}", CurrentUserId);

        if (string.IsNullOrEmpty(CurrentUserId))
        {
            return Unauthorized(new ApiResponse
            {
                Success = false,
                Message = "User not authenticated",
            });
        }

        //var user = await userService.GetUserByIdAsync(CurrentUserId);
        //if (user is null)
        //{
        //    return BadRequest(new ApiResponse
        //    {
        //        Success = false, ErrorCode = Application.Exceptions.ErrorCode.UserNotFound, Message = "User not registered",
        //    });
        //}

        var gameId = await gameService.CreateGameAsync(CurrentUserId);
        logger.LogInformation("Game created with ID: {GameId}", gameId);

        return Ok(new ApiResponse<CreateGameResponse>
        {
            Data = new CreateGameResponse
            {
                GameId = gameId,
            },
        });
    }

    [HttpPost("join/{gameId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> JoinGame(Guid gameId)
    {
        await gameService.AddPlayerToGameAsync(gameId, CurrentUserId);
        await gameEventService.BroadcastEventAsync(gameId, "UserJoinedLobby", $"{CurrentUserId} successfully joined the game");

        return Ok(new ApiResponse
        {
            Message = $"{CurrentUserId} successfully joined the game",
        });
    }

    [HttpPost("start/{gameId:guid}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse<StartGameResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StartGame(Guid gameId)
    {
        await gameService.StartGameAsync(gameId, CurrentUserId);

        var response = new StartGameResponse
        {
            GameId = gameId,
            NextActivePlayer = "n/a",
            NextWord = "n/a",
            MethodType = MethodType.Description.ToString(),
        };

        await gameEventService.BroadcastEventAsync(gameId, "GameStarted", "Game started successfully");
        logger.LogInformation("Game {GameId} started", gameId);

        //TODO startgame response not needed? Fropntend doesnt use any properties? Investigate

        return Ok(new ApiResponse<StartGameResponse>
        {
            Data = response,
            Message = "Game started successfully",
        });
    }

    [HttpPost("end-turn/{gameId:guid}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse<EndTurnResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EndTurn(Guid gameId, [FromBody] EndTurnRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var isGameWon = await gameService.EndTurnAsync(gameId, CurrentUserId, request.WinnerUserId);

        var response = new EndTurnResponse
        {
            IsGameWon = isGameWon,
        };

        if (!isGameWon)
        {
            await gameEventService.BroadcastEventAsync(gameId, "RoundEnded", "Round ended for game");
            logger.LogInformation("Round ended for game {GameId}", gameId);
        }
        else
        {
            await gameEventService.BroadcastEventAsync(gameId, "GameEnded", "Game ended");
            logger.LogInformation("Game {GameId} ended. Winner: {WinnerId}", gameId, request.WinnerUserId);
        }

        //TODO Use IsGameFinished instead of IsGameWon? Or return ApiResponse -> Frontend doesnt use isgamewon property, get the info from sse event
        return Ok(new ApiResponse<EndTurnResponse>
        {
            Success = true,
            Data = response,
            Message = isGameWon ? "Game ended" : "Turn ended successfully",
        });
    }

    [HttpPost("leave-lobby/{gameId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LeaveLobby(Guid gameId)
    {
        var userId = CurrentUserId;

        await gameService.RemovePlayerFromLobbyAsync(gameId, userId);
        await gameEventService.BroadcastEventAsync(gameId, "PlayerLeftLobby", $"User {userId} left the lobby");

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "User left the lobby successfully.",
        });
    }

    [HttpPost("leave/{gameId}")]
    public async Task<IActionResult> LeaveGame(Guid gameId)
    {
        var userId = identityService.GetId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new ApiResponse
            {
                Success = false,
                Message = "User not authenticated",
            });
        }

        var isGameOver = await gameService.LeaveGameAsync(gameId, userId);

        if (isGameOver)
        {
            await gameEventService.BroadcastEventAsync(gameId, "GameEnded", $"User {userId} left the lobby, game ended");
        }
        else
        {
            await gameEventService.BroadcastEventAsync(gameId, "PlayerLeftLobby", $"User {userId} left the lobby");
        }

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Left the game successfully",
        });
    }

    [HttpPut("settings/{gameId:guid}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateGameSettings(Guid gameId, [FromBody] UpdateGameSettingsRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        await gameService.UpdateGameSettingsAsync(gameId, request.Timer, request.MaxScore, request.EnabledMethods);

        await gameEventService.BroadcastEventAsync(gameId, "GameSettingsUpdated", $"Game settings just updated.");

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Game settings updated successfully",
        });
    }
}
