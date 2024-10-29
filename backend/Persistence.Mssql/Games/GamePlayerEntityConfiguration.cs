using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ActivityGameBackend.Persistence.Mssql.Games;

public class GamePlayerEntityConfiguration : IEntityTypeConfiguration<GamePlayerEntity>
{
    public void Configure(EntityTypeBuilder<GamePlayerEntity> builder)
    {
        builder.HasKey(gp => new { gp.GameId, gp.UserId });

        builder.HasOne(gp => gp.Game)
            .WithMany(g => g.GamePlayers)
            .HasForeignKey(gp => gp.GameId);

        builder.HasOne(gp => gp.User)
            .WithMany(u => u.GamePlayers)
            .HasForeignKey(gp => gp.UserId);

        builder.Property(gp => gp.Score)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(gp => gp.IsHost)
            .IsRequired()
            .HasDefaultValue(false);
    }
}
