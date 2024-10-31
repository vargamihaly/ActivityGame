using System.Security.Claims;
using System.Text.Json;
using ActivityGameBackend.Application.Users;
using ActivityGameBackend.Web.Controllers.Authentication.Request;
using ActivityGameBackend.Web.Controllers.Authentication.Response;
using ActivityGameBackend.Web.Services;
using ActivityGameBackend.Web.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace ActivityGameBackend.Web.Controllers.Authentication;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuthController(IUserService userService, IIdentityService identityService, IHttpClientFactory httpClientFactory) : ControllerBase
{
    private static readonly Uri GoogleUserInfoEndpoint = new("https://www.googleapis.com/oauth2/v3/userinfo");

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromHeader(Name = "Authorization")] string authorization)
    {
        if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new ApiResponse { Success = false, Message = "Invalid authorization header" });

        var token = authorization["Bearer ".Length..].Trim();

        try
        {
            using var userInfo = await GetGoogleUserInfoAsync(token);
            var user = await GetOrCreateUserAsync(userInfo);
            await SignInUserAsync(user);

            return Ok(new ApiResponse<UserResponse>
            {
                Success = true,
                Data = new UserResponse()
                {
                    Id = user.Id,
                    Email = user.Email,
                    IsHost = user.IsHost,
                    Score = user.Score,
                    Username = user.Username,
                },
            });
        }
        catch (HttpRequestException ex)
        {
            return BadRequest(new ApiResponse { Success = false, Message = $"Error validating Google token: {ex.Message}" });
        }
        catch (JsonException ex)
        {
            return BadRequest(new ApiResponse { Success = false, Message = $"Error parsing user info: {ex.Message}" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse { Success = false, Message = $"An unexpected error occurred: {ex.Message}" });
        }
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var id = identityService.GetId();
        if (string.IsNullOrEmpty(id))
            return Unauthorized(new ApiResponse { Success = false, Message = "User not authenticated" });

        var user = await userService.GetUserByIdAsync(id);
        if (user != null)
        {
            return Ok(new ApiResponse<UserResponse>
            {
                Success = true,
                Data = new UserResponse()
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.Username,
                    Score = user.Score,
                    IsHost = user.IsHost,
                },
            });
        }

        return NotFound(new ApiResponse { Success = false, Message = "User not found" });
    }

    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new ApiResponse { Success = true, Message = "Logged out successfully" });
    }

    [HttpPost("username")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetUsername([FromBody] SetUsernameRequest? request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Username))
            return BadRequest(new ApiResponse { Success = false, Message = "Invalid username request" });

        var id = identityService.GetId();
        if (string.IsNullOrEmpty(id))
            return Unauthorized(new ApiResponse { Success = false, Message = "User not authenticated" });

        var user = await userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound(new ApiResponse { Success = false, Message = "User not found" });

        user.Username = request.Username;
        await userService.UpdateUserAsync(user);
        return Ok(new ApiResponse<UserResponse>
        {
            Success = true,
            Data = new UserResponse()
            {
                Id = user.Id,
                Email = user.Email,
                IsHost = user.IsHost,
                Score = user.Score,
                Username = user.Username,
            },
        });
    }

    private async Task<JsonDocument> GetGoogleUserInfoAsync(string token)
    {
        using var httpClient = httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        using var response = await httpClient.GetAsync(GoogleUserInfoEndpoint);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(content);
    }

    private async Task<User> GetOrCreateUserAsync(JsonDocument userInfo)
    {
        var root = userInfo.RootElement;
        var userId = root.GetProperty("sub").GetString() ?? throw new ArgumentException("User ID is missing", nameof(userInfo));
        var email = root.GetProperty("email").GetString() ?? throw new ArgumentException("Email is missing", nameof(userInfo));
        var name = root.GetProperty("name").GetString() ?? throw new ArgumentException("Name is missing", nameof(userInfo));

        var existingUser = await userService.GetUserByIdAsync(userId);
        return existingUser ?? await userService.CreateUserAsync(userId, email, name);
    }

    private async Task SignInUserAsync(User user)
    {
        var claims = new List<Claim>
    {
        new (ClaimTypes.NameIdentifier, user.Id),
        new (ClaimTypes.Email, user.Email),
        new (ClaimTypes.Name, user.Username),
    };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
    }
}
