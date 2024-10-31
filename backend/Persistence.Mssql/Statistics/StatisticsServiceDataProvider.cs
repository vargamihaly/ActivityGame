using ActivityGameBackend.Application.Statistics;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ActivityGameBackend.Persistence.Mssql.Statistics;

public class StatisticsServiceDataProvider(ApplicationDbContext context, IMapper mapper) : IStatisticsServiceDataProvider
{

    public async Task<List<Game>> GetFinishedGamesAsync()
    {
        var gameEntities =  await context.Games
            .Include(g => g.GamePlayers)
            .ThenInclude(p => p.User)
            .Where(g => g.Status == GameStatus.Finished)
            .ToListAsync();
        
        return mapper.Map<List<Game>>(gameEntities);
    }
    
    public async Task<List<Game>> GetGamesForUserAsync(string userId)
    {
        var gameEntities = await context.Games
            .Include(g => g.GamePlayers)
            .Where(g => g.GamePlayers.Any(p => p.UserId == userId))
            .ToListAsync();
        
        return mapper.Map<List<Game>>(gameEntities);
    }
}
