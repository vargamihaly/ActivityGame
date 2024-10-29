using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ActivityGameBackend.Persistence.Mssql.Games;

public class GameEntityConfiguration : IEntityTypeConfiguration<GameEntity>
{
    public void Configure(EntityTypeBuilder<GameEntity> builder)
    {
        builder.HasKey(g => g.Id);

        builder.HasOne(g => g.Host)
            .WithMany()
            .HasForeignKey(g => g.HostId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(g => g.Winner)
            .WithMany()
            .HasForeignKey(g => g.WinnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(g => g.GamePlayers)
            .WithOne(gp => gp.Game)
            .HasForeignKey(gp => gp.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(g => g.Status)
            .IsRequired();

        builder.Property(g => g.TimerInMinutes)
            .IsRequired();

        builder.Property(g => g.MaxScore)
            .IsRequired();

        builder.Property(g => g.EnabledMethods)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(Enum.Parse<MethodType>).ToList());
    }
}
