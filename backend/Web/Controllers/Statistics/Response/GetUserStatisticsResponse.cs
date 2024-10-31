namespace ActivityGameBackend.Web.Controllers.Statistics.Response;

public sealed record GetUserStatisticsResponse
{
    public string Username { get; set; }
    public int GamesPlayed { get; set; }
    public int GamesWon { get; set; }
    public int GamesLost { get; set; }
    public double WinRate { get; set; } // Percentage
    public double AverageScore { get; set; }
}