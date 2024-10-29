using ActivityGameBackend.Application.Exceptions;
using ActivityGameBackend.Application.Helpers;

namespace ActivityGameBackend.Application.Users;

public interface IUserService : IScoped
{
    Task<User?> GetUserAsync(string id);
    Task<User> CreateUserAsync(string id, string email, string userName);
    Task<User> UpdateUserAsync(User user);
}

//TODO DELETE this and move to gameservice
public class UserService(IUserServiceDataProvider dataProvider) : IUserService
{
    public async Task<User> CreateUserAsync(string id, string email, string userName)
    {
        var user = new User
        {
            Id = id,
            Email = email,
            Username = userName,
            Score = 0,
            IsHost = false,
        };

        return await dataProvider.CreateUserAsync(user);
    }

    public async Task<User?> GetUserAsync(string id) => await dataProvider.GetUserAsync(id);
    public async Task<User> UpdateUserAsync(User user) => (await dataProvider.UpdateUserAsync(user)).EnsureNotNull(() => throw new UserNotFoundException(user.Id));
}
