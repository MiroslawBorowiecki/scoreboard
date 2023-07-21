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
        if (string.IsNullOrWhiteSpace(homeTeam)) throw new ArgumentException(null, nameof(homeTeam));
        if (string.IsNullOrWhiteSpace(awayTeam)) throw new ArgumentException(null, nameof(awayTeam));

        var conflictingMatch = _matches.CheckForConflictingMatches(homeTeam, awayTeam);
        if (conflictingMatch != null) throw new ArgumentException(
            "Cannot start a new match - one of the teams is already playing: " +
            $"{MatchScoreModel.ToMatchScore(conflictingMatch)}");

        var match = MatchScoreModel.Create(homeTeam, awayTeam);
        _matches.Add(match);
        return MatchScoreModel.ToMatchScore(match);
    }

    public IEnumerable<MatchScore> GetSummary() => 
        _matches.GetMatchesOrderedByTotalScoreWithRecentFirst()
        .Select(MatchScoreModel.ToMatchScore);

    public void UpdateScore(Guid matchId, int homeScore, int awayScore)
    {
        if (matchId == Guid.Empty) throw new ArgumentException(null, nameof(matchId));

        if (homeScore < 0) throw new ArgumentOutOfRangeException(nameof(homeScore));
        if (awayScore < 0) throw new ArgumentOutOfRangeException(nameof(awayScore));

        if (!_matches.UpdateScore(matchId, homeScore, awayScore))
            throw new ArgumentException($"{MatchNotFoundMessage}{matchId}");
    }

    public void FinishMatch(Guid matchId)
    {
        if (matchId == Guid.Empty) throw new ArgumentException(null, nameof(matchId));

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