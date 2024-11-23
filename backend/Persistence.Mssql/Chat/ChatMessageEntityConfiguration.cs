using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ActivityGameBackend.Persistence.Mssql.Chat;

public class ChatMessageEntityConfiguration : IEntityTypeConfiguration<ChatMessageEntity>
{
    public void Configure(EntityTypeBuilder<ChatMessageEntity> builder)
    {
        builder.HasKey(cm => cm.Id);

        builder.HasOne(cm => cm.Game)
            .WithMany(g => g.ChatMessages)
            .HasForeignKey(cm => cm.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cm => cm.Sender)
            .WithMany()
            .HasForeignKey(cm => cm.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(cm => cm.Content)
            .IsRequired();

        builder.Property(cm => cm.Timestamp)
            .IsRequired();
    }
}
