using Scoreboard.Domain;
using Scoreboard.Infrastructure;

namespace Scoreboard;

public class Scoreboard
{
    public const string MatchNotFoundMessage = "Match not found. ID: ";
    private readonly IMatchScoreRepository _matches;

    internal Scoreboard(IMatchScoreRepository matches)
    {
        _matches = matches;
    }

    public MatchScore StartNewMatch(string homeTeam, string awayTeam)
    {
        Rules.TeamNameMustContainCharacters(homeTeam, nameof(homeTeam));
        Rules.TeamNameMustContainCharacters(awayTeam, nameof(awayTeam));

        Rules.TeamsCannotBePlayingAnotherMatch(_matches, homeTeam, awayTeam);

        var match = MatchScoreModel.Create(homeTeam, awayTeam);
        _matches.Add(match);
        return MatchScoreModel.ToMatchScore(match);
    }

    public IEnumerable<MatchScore> GetSummary() => 
        _matches.GetMatchesOrderedByTotalScoreWithRecentFirst()
        .Select(MatchScoreModel.ToMatchScore);

    public void UpdateScore(Guid matchId, int homeScore, int awayScore)
    {
        Rules.MatchIdCannotBeEmtpy(matchId);

        Rules.ScoreCannotBeNegative(homeScore, awayScore);

        if (!_matches.UpdateScore(matchId, homeScore, awayScore))
            throw new ArgumentException($"{MatchNotFoundMessage}{matchId}");
    }

    public void FinishMatch(Guid matchId)
    {
        Rules.MatchIdCannotBeEmtpy(matchId);

        if (!_matches.Remove(matchId))
            throw new ArgumentException($"{MatchNotFoundMessage}{matchId}");
    }
}

public record MatchScore(
    Guid Id,
    string HomeTeam, 
    string AwayTeam, 
    int HomeTeamScore, 
    int AwayTeamScore
    );

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