using System.Net.Mime;
using ActivityGameBackend.Application.Games;
using ActivityGameBackend.Application.Users;
using ActivityGameBackend.Web.Controllers.Game.Request;
using ActivityGameBackend.Web.Controllers.Game.Response;
using ActivityGameBackend.Web.Services;
using ActivityGameBackend.Web.Shared;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;

namespace ActivityGameBackend.Web.Controllers.Game;

/// <summary>
/// Controller for managing game operations including creation, joining, and gameplay actions
/// </summary>
[ApiController]
[Route("api/games")] // Changed to follow Google Cloud API Design Guide
[Authorize]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Manage game operations and player interactions")]
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

    /// <summary>
    /// Retrieves the current active game for the authenticated user
    /// </summary>
    /// <remarks>
    /// Returns the active game details if the user is currently in a game, otherwise returns null
    /// </remarks>
    /// <response code="200">Successfully retrieved game information</response>
    /// <response code="401">User is not authenticated</response>
    [HttpGet("current")]
    [SwaggerOperation(
        Summary = "Get current active game",
        Description = "Retrieves the current active game for the authenticated user")]
    [ProducesResponseType(typeof(ApiResponse<GameResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentGame()
    {
        var userId = identityService.GetId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new ApiResponse
            {
                Success = false, Message = "User not authenticated",
            });
        }

        var game = await gameService.GetActiveGameForPlayerAsync(userId);
        if (game == null)
        {
            return Ok(new ApiResponse<GameResponse>
            {
                Success = true, Data = null,
            });
        }

        var gameResponse = mapper.Map<GameResponse>(game);
        return Ok(new ApiResponse<GameResponse>
        {
            Success = true, Data = gameResponse,
        });
    }

    /// <summary>
    /// Retrieves detailed information about a specific game
    /// </summary>
    /// <param name="gameId">The unique identifier of the game</param>
    [HttpGet("{gameId}")] // Changed from "details/{gameId:guid}" to follow Google Cloud API Design Guide
    [SwaggerOperation(Summary = "Get game details", Description = "Retrieves detailed information about a specific game")]
    [ProducesResponseType(typeof(ApiResponse<GameResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGameDetails(
        [FromRoute, SwaggerParameter("The unique identifier of the game")]
        Guid gameId)
    {
        var game = await gameService.GetGameDetailsAsync(gameId);

        var gameResponse = mapper.Map<GameResponse>(game);
        return Ok(new ApiResponse<GameResponse>
        {
            Success = true, Data = gameResponse,
        });
    }

    /// <summary>
    /// Creates a new game instance
    /// </summary>
    /// <remarks>
    /// Creates a new game and assigns the authenticated user as the host
    /// </remarks>
    [HttpPost] // Changed from "create" to follow Google Cloud API Design Guide
    [SwaggerOperation(
        Summary = "Create new game",
        Description = "Creates a new game instance with the current user as host")]
    [ProducesResponseType(typeof(ApiResponse<CreateGameResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateGame()
    {
        logger.LogInformation("Create Game endpoint hit for user {UserId}", CurrentUserId);

        if (string.IsNullOrEmpty(CurrentUserId))
        {
            return Unauthorized(new ApiResponse
            {
                Success = false, Message = "User not authenticated",
            });
        }

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

    /// <summary>
    /// Adds a player to an existing game
    /// </summary>
    /// <param name="gameId">The unique identifier of the game to join</param>
    [HttpPost("{gameId}/join")]
    [SwaggerOperation(
        Summary = "Join existing game",
        Description = "Adds the current user as a player to an existing game")]
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

    /// <summary>
    /// Initiates the start of a game
    /// </summary>
    /// <param name="gameId">The unique identifier of the game to start</param>
    [HttpPost("{gameId}/start")]
    [SwaggerOperation(Summary = "Start game", Description = "Initiates the start of a game, transitioning it from lobby to active state")]
    [ProducesResponseType(typeof(ApiResponse<StartGameResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StartGame(
        [FromRoute, SwaggerParameter("The unique identifier of the game to start")]
        Guid gameId)
    {
        await gameService.StartGameAsync(gameId, CurrentUserId);

        var response = new StartGameResponse
        {
            GameId = gameId, NextActivePlayer = "n/a", NextWord = "n/a", MethodType = MethodType.Description.ToString(),
        };

        await gameEventService.BroadcastEventAsync(gameId, "GameStarted", "Game started successfully");
        logger.LogInformation("Game {GameId} started", gameId);

        //TODO startgame response not needed? Fropntend doesnt use any properties? Investigate

        return Ok(new ApiResponse<StartGameResponse>
        {
            Data = response, Message = "Game started successfully",
        });
    }

    /// <summary>
    /// Ends the current turn
    /// </summary>
    /// <param name="gameId">The unique identifier of the game</param>
    /// <param name="request">The end turn request containing the winner information</param>
    [HttpPost("{gameId}/turns/end")]
    [SwaggerOperation(
        Summary = "End current turn",
        Description = "Ends the current player's turn and processes the turn results")]
    [ProducesResponseType(typeof(ApiResponse<EndTurnResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EndTurn(
        [FromRoute, SwaggerParameter("The unique identifier of the game")]
        Guid gameId,
        [FromBody, SwaggerRequestBody("The end turn request details")]
        EndTurnRequest request)
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
            Success = true, Data = response, Message = isGameWon ? "Game ended" : "Turn ended successfully",
        });
    }

    /// <summary>
    /// Removes a player from the game lobby
    /// </summary>
    /// <param name="gameId">The unique identifier of the game</param>
    [HttpPost("{gameId}/lobby/leave")]
    [SwaggerOperation(Summary = "Leave game lobby", Description = "Removes the current player from the game lobby")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LeaveLobby(
        [FromRoute, SwaggerParameter("The unique identifier of the game")]
        Guid gameId)
    {
        var userId = CurrentUserId;

        await gameService.RemovePlayerFromLobbyAsync(gameId, userId);
        await gameEventService.BroadcastEventAsync(gameId, "PlayerLeftLobby", $"User {userId} left the lobby");

        return Ok(new ApiResponse
        {
            Success = true, Message = "User left the lobby successfully.",
        });
    }

    /// <summary>
    /// Removes a player from an active game
    /// </summary>
    /// <param name="gameId">The unique identifier of the game</param>
    [HttpPost("{gameId}/leave")]
    [SwaggerOperation(
        Summary = "Leave active game",
        Description = "Removes the current player from an active game")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LeaveGame(
        [FromRoute, SwaggerParameter("The unique identifier of the game")]
        Guid gameId)
    {
        var userId = identityService.GetId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new ApiResponse
            {
                Success = false, Message = "User not authenticated",
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
            Success = true, Message = "Left the game successfully",
        });
    }

    /// <summary>
    /// Updates the settings for a game
    /// </summary>
    /// <param name="gameId">The unique identifier of the game</param>
    /// <param name="request">The updated game settings</param>
    [HttpPatch("{gameId}/settings")]
    [SwaggerOperation(Summary = "Update game settings", Description = "Updates the settings for an existing game")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateGameSettings(
        [FromRoute, SwaggerParameter("The unique identifier of the game")]
        Guid gameId,
        [FromBody, SwaggerRequestBody("The updated game settings")]
        UpdateGameSettingsRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        await gameService.UpdateGameSettingsAsync(gameId, request.Timer, request.MaxScore, request.EnabledMethods);
        await gameEventService.BroadcastEventAsync(gameId, "GameSettingsUpdated", $"Game settings just updated.");

        return Ok(new ApiResponse
        {
            Success = true, Message = "Game settings updated successfully",
        });
    }

    /// <summary>
    /// Handles the time-up event for a game round
    /// </summary>
    /// <param name="gameId">The unique identifier of the game</param>
    [HttpPost("{gameId}/time-up")]
    [SwaggerOperation(Summary = "Handle time up event", Description = "Processes the time-up event for a game round")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> HandleTimeUp(
        [FromRoute, SwaggerParameter("The unique identifier of the game")]
        Guid gameId)
    {
        await gameService.HandleTimeUpAsync(gameId);
        await gameEventService.BroadcastEventAsync(gameId, "TimeUp", "Round time is up!");

        return Ok(new ApiResponse
        {
            Success = true, Message = "Time up handled successfully",
        });
    }
}
