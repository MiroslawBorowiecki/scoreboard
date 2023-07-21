namespace Scoreboard.Infrastructure;

internal class MatchScoreModel
{
    protected MatchScoreModel() { }

    public Guid Id { get; set; }
    public string HomeTeam { get; set; } = null!;
    public string AwayTeam { get; set; } = null!;
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }

    public DateTime? Added { get; set; }

    public static MatchScore ToMatchScore(MatchScoreModel model)
        => new(model.Id, model.HomeTeam!, model.AwayTeam!, model.HomeScore, model.AwayScore);

    public static MatchScoreModel Create(string homeTeam, string awayTeam) => new()
    {
        Id = Guid.NewGuid(),
        HomeTeam = homeTeam,
        AwayTeam = awayTeam,
        HomeScore = 0,
        AwayScore = 0,
        Added = DateTime.UtcNow
    };
}