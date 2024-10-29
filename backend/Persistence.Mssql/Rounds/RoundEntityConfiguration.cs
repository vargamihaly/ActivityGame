using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ActivityGameBackend.Persistence.Mssql.Rounds;
public sealed class RoundEntityConfiguration : IEntityTypeConfiguration<RoundEntity>
{
    public void Configure(EntityTypeBuilder<RoundEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Rounds");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.HasOne(r => r.Game)
            .WithMany(g => g.Rounds)
            .HasForeignKey(r => r.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.RoundWinner)
            .WithMany()
            .HasForeignKey(r => r.RoundWinnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Word)
            .WithMany()
            .HasForeignKey(r => r.WordId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.ActivePlayer)
            .WithMany()
            .HasForeignKey(r => r.ActivePlayerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(r => r.MethodType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(m => m.CreatedAtUtc)
            .IsRequired();
    }
}