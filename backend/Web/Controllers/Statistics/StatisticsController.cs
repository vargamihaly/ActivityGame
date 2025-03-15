using ActivityGameBackend.Application.Statistics;
using ActivityGameBackend.Web.Controllers.Statistics.Response;
using ActivityGameBackend.Web.Services;
using ActivityGameBackend.Web.Shared;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System.Net.Mime;

namespace ActivityGameBackend.Web.Controllers.Statistics;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatisticsController(
    IMapper mapper,
    IIdentityService identityService,
    IStatisticsService statisticsService)
    : ControllerBase
{
    private string CurrentUserId => identityService.GetId();

    [HttpGet("statistics/global")]
    [AllowAnonymous]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse<GetGlobalStatisticsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGlobalStatistics()
    {
        var statistics = await statisticsService.GetGlobalStatisticsAsync();

        return Ok(new ApiResponse<GetGlobalStatisticsResponse>
        {
            Success = true, Data = mapper.Map<GetGlobalStatisticsResponse>(statistics),
        });
    }

    [HttpGet("statistics/user")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse<GetUserStatisticsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserStatistics()
    {
        var userId = identityService.GetId();

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new ApiResponse
            {
                Success = false, Message = "User not authenticated",
            });
        }

        var statistics = await statisticsService.GetUserStatisticsAsync(userId);
        return Ok(new ApiResponse<GetUserStatisticsResponse>
        {
            Success = true, Data = mapper.Map<GetUserStatisticsResponse>(statistics),
        });
    }
}
