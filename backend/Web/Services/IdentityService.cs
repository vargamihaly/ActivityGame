using ActivityGameBackend.Common.Core;
using System.Security.Claims;

namespace ActivityGameBackend.Web.Services;

public interface IIdentityService : IScoped
{
    string GetId();

    string GetEmail();

    string GetUsername();
}

public class IdentityService(IHttpContextAccessor httpContextAccessor) : IIdentityService
{
    public string GetId()
    {
#pragma warning disable CS8603 // Possible null reference return.
        return httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
#pragma warning restore CS8603 // Possible null reference return.
    }

    public string GetEmail()
    {
#pragma warning disable CS8603 // Possible null reference return.
        return httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);
#pragma warning restore CS8603 // Possible null reference return.
    }

    public string GetUsername()
    {
#pragma warning disable CS8603 // Possible null reference return.
        return httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
#pragma warning restore CS8603 // Possible null reference return.
    }
}
