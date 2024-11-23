using ActivityGameBackend.Persistence.Mssql.Chat;
using ActivityGameBackend.Persistence.Mssql.Games;
using Microsoft.EntityFrameworkCore;

namespace ActivityGameBackend.Persistence.Mssql;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<GameEntity> Games { get; set; }
    public DbSet<RoundEntity> Rounds { get; set; }
    public DbSet<WordEntity> Words { get; set; }
    public DbSet<GamePlayerEntity> GamePlayers { get; set; }
    public DbSet<ChatMessageEntity> ChatMessages { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
