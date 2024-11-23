// File: Services/GameEventService.cs

using ActivityGameBackend.Application.Chat;
using ActivityGameBackend.Common.Core;
using Newtonsoft.Json;
using System.Collections.Concurrent;

public interface IGameEventService : ISingleton
{
    void AddClient(Guid gameId, string clientId, Func<string, Task> sendEvent);
    void RemoveClient(Guid gameId, string clientId);
    Task BroadcastEventAsync(Guid gameId, string eventName, object eventData);
}

public class GameEventService : IGameEventService
{
    private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<string, Func<string, Task>>> gameClients =
        new ConcurrentDictionary<Guid, ConcurrentDictionary<string, Func<string, Task>>>();

    public void AddClient(Guid gameId, string clientId, Func<string, Task> sendEvent)
    {
        var clients = gameClients.GetOrAdd(gameId, _ => new ConcurrentDictionary<string, Func<string, Task>>());
        clients[clientId] = sendEvent;
    }

    public void RemoveClient(Guid gameId, string clientId)
    {
        if (gameClients.TryGetValue(gameId, out var clients))
        {
            clients.TryRemove(clientId, out _);
            if (clients.IsEmpty)
            {
                gameClients.TryRemove(gameId, out _);
            }
        }
    }

    public async Task BroadcastEventAsync(Guid gameId, string eventName, object eventData)
    {
        if (gameClients.TryGetValue(gameId, out var clients))
        {
            var message = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                @event = eventName,
                data = eventData,
            });

            var tasks = new List<Task>();
            foreach (var sendEvent in clients.Values)
            {
                tasks.Add(sendEvent(message));
            }

            await Task.WhenAll(tasks);
        }
    }
}
