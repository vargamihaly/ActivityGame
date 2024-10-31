namespace ActivityGameBackend.Application.Users;

public interface IUserServiceDataProvider : IScoped
{
    Task<User?> GetUserByIdAsync(string id);
    Task<User> CreateUserAsync(User user);
    Task<User> UpdateUserAsync(User user);
}
