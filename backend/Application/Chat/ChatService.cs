// using Microsoft.Extensions.Logging;
//
// namespace ActivityGameBackend.Application.Chat;
//
// public interface IChatService : IScoped
// {
//     Task<ChatMessage> SaveMessageAsync(Guid gameId, string senderId, string message);
//     Task<List<ChatMessage>> GetGameChatHistoryAsync(Guid gameId);
// }
//
// public class ChatService(IChatServiceDataProvider dataProvider, ILogger<ChatService> logger) : IChatService
// {
//     public async Task<ChatMessage> SaveMessageAsync(Guid gameId, string senderId, string message)
//     {
//         try 
//         {
//             logger.LogDebug("Saving chat message for game {GameId} from sender {SenderId}", gameId, senderId);
//             var savedMessage = await dataProvider.SaveMessageAsync(gameId, senderId, message);
//             logger.LogDebug("Message saved successfully with ID {MessageId}", savedMessage.Id);
//             return savedMessage;
//         }
//         catch (Exception ex)
//         {
//             logger.LogError(ex, "Failed to save chat message for game {GameId} from sender {SenderId}", gameId, senderId);
//             throw;
//         }
//     }
//
//     public async Task<List<ChatMessage>> GetGameChatHistoryAsync(Guid gameId)
//     {
//         try
//         {
//             logger.LogDebug("Retrieving chat history for game {GameId}", gameId);
//             var history = await dataProvider.GetChatHistoryAsync(gameId);
//             logger.LogDebug("Retrieved {MessageCount} messages for game {GameId}", history.Count, gameId);
//             return history;
//         }
//         catch (Exception ex)
//         {
//             logger.LogError(ex, "Failed to retrieve chat history for game {GameId}", gameId);
//             throw;
//         }
//     }
// }
