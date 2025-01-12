using ActivityGameBackend.Application.Chat;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace ActivityGameBackend.Persistence.Mssql.Chat;

public class ChatServiceDataProvider(ApplicationDbContext context, IMapper mapper) : IChatServiceDataProvider
{
    public async Task<ChatMessage> SaveMessageAsync(Guid gameId, string senderId, string message)
    {
        var chatMessageEntity = new ChatMessageEntity
        {
            GameId = gameId,
            SenderId = senderId,
            Content = message,
            Timestamp = DateTimeOffset.UtcNow,
        };

        context.ChatMessages.Add(chatMessageEntity);

        await context.SaveChangesAsync().ConfigureAwait(false);

        return mapper.Map<ChatMessage>(chatMessageEntity);
    }

    public async Task<List<ChatMessage>> GetChatHistoryAsync(Guid gameId)
    {
        return await context.ChatMessages
            .Where(x => x.GameId == gameId)
            .OrderBy(x => x.Timestamp)
            .ProjectTo<ChatMessage>(mapper.ConfigurationProvider)
            .ToListAsync();
    }
}


