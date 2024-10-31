namespace ActivityGameBackend.Application.Statistics;

public class GlobalStatistics
{
    public double AverageScore { get; set; }
    public double WinRate { get; set; }  // Percentage
    public double LossRate { get; set; } // Percentage
    public List<PlayerRanking> PlayerRankings { get; set; } = new();
}
