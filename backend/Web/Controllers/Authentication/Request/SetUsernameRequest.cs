namespace ActivityGameBackend.Web.Controllers.Authentication.Request
{
    public sealed record SetUsernameRequest()
    {
        required public string Username { get; init; }
    }
}
