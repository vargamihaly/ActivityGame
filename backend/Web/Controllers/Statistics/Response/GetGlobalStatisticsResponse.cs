namespace ActivityGameBackend.Web.Controllers.Statistics.Response;

public sealed record GetGlobalStatisticsResponse
{
    public double AverageScore { get; set; }
    public double WinRate { get; set; }
    public double LossRate { get; set; }
    public List<PlayerRankingResponse> PlayerRankings { get; set; } = new();
}

public sealed record PlayerRankingResponse
{
    public string Username { get; set; }
    public int TotalScore { get; set; }
}
