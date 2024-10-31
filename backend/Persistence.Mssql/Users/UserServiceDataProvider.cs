using ActivityGameBackend.Application.Users;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ActivityGameBackend.Persistence.Mssql.Users;

public class UserServiceDataProvider(ApplicationDbContext context, IMapper mapper) : IUserServiceDataProvider
{

    public async Task<User?> GetUserByIdAsync(string id)
    {
        var userEntity = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
        return mapper.Map<User>(userEntity);
    }

    public async Task<User> CreateUserAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var userEntity = new UserEntity
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            CreatedAtUtc = DateTimeOffset.UtcNow,
        };

        context.Users.Add(userEntity);
        await context.SaveChangesAsync();

        return mapper.Map<User>(userEntity);
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var userEntity = await context.Users.FindAsync(user.Id) ?? throw new ArgumentException("User not found", nameof(user));
        userEntity.Email = user.Email;
        userEntity.Username = user.Username;

        await context.SaveChangesAsync();

        return mapper.Map<User>(userEntity);
    }
}
