// File: Controllers/GameEventsController.cs

namespace ActivityGameBackend.Web.Controllers.SignalR
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameEventsController : ControllerBase
    {
        private readonly IGameEventService gameEventService;
        private readonly ILogger<GameEventsController> logger;

        public GameEventsController(IGameEventService gameEventService, ILogger<GameEventsController> logger)
        {
            this.gameEventService = gameEventService;
            this.logger = logger;
        }

        [HttpGet("{gameId}")]
        public async Task InitiateGameEventStream(Guid gameId)
        {
            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");
            Response.Headers.Add("Access-Control-Allow-Origin", "*");

            var clientId = Guid.NewGuid().ToString();
            var cancellationToken = HttpContext.RequestAborted;

            try
            {
                // Register the client
                gameEventService.AddClient(gameId, clientId, async (data) =>
                {
                    try
                    {
                        var eventData = $"data: {data}\n\n";
                        await Response.WriteAsync(eventData);
                        await Response.Body.FlushAsync();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error sending event data to client");
                    }
                });

                // Send initial connection message
                await Response.WriteAsync($": connected\n\n");
                await Response.Body.FlushAsync();

                // Keep the connection alive
                while (!cancellationToken.IsCancellationRequested)
                {
                    // Send keep-alive comments to prevent connection from closing
                    await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);
                    await Response.WriteAsync(": keep-alive\n\n");
                    await Response.Body.FlushAsync();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in SSE connection");
            }
            finally
            {
                gameEventService.RemoveClient(gameId, clientId);
                logger.LogInformation("Client {ClientId} disconnected from game {GameId}", clientId, gameId);
            }
        }
    }
}
