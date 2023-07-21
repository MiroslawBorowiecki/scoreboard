using Scoreboard.Infrastructure;

namespace Scoreboard;

public class Scoreboard
{
    public const string MatchNotFoundMessageFormat = "Match not found. ID: {0};";
    private readonly IMatchScoreRepository _scores;

    internal Scoreboard(IMatchScoreRepository scores)
    {
        _scores = scores;
    }

    public MatchScore StartNewMatch(string homeTeam, string awayTeam)
    {
        if (string.IsNullOrWhiteSpace(homeTeam)) throw new ArgumentException(null, nameof(homeTeam));
        if (string.IsNullOrWhiteSpace(awayTeam)) throw new ArgumentException(null, nameof(awayTeam));

        var conflictingMatch = _scores.CheckForConflictingMatches(homeTeam, awayTeam);
        if (conflictingMatch != null) throw new ArgumentException(
            "Cannot start a new match - one of the teams is already playing: " +
            $"{MatchScoreModel.ToMatchScore(conflictingMatch)}");

        var matchScore = new MatchScore(Guid.NewGuid(), homeTeam, awayTeam, 0, 0);
        _scores.Add(MatchScoreModel.FromMatchScore(matchScore));
        return matchScore;
    }

    public IEnumerable<MatchScore> GetSummary() => 
        _scores.GetMatchesOrderedByTotalScoreWithRecentFirst()
        .Select(MatchScoreModel.ToMatchScore);

    public void UpdateScore(Guid matchId, int homeScore, int awayScore)
    {
        if (matchId == Guid.Empty) throw new ArgumentException(null, nameof(matchId));

        if (homeScore < 0) throw new ArgumentOutOfRangeException(nameof(homeScore));
        if (awayScore < 0) throw new ArgumentOutOfRangeException(nameof(awayScore));

        if (!_scores.UpdateScore(matchId, homeScore, awayScore))
            throw new ArgumentException(string.Format(MatchNotFoundMessageFormat, matchId.ToString()));
    }

    public void FinishMatch(Guid matchId)
    {
        if (matchId == Guid.Empty) throw new ArgumentException(null, nameof(matchId));

        if (!_scores.Remove(matchId)) throw new ArgumentException(
            string.Format(MatchNotFoundMessageFormat, matchId.ToString()));
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
    public Guid Id { get; set; }
    public string HomeTeam { get; set; } = null!;
    public string AwayTeam { get; set; } = null!;
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }

    public DateTime? Added { get; set; }

    public static MatchScoreModel FromMatchScore(MatchScore score)
    {
        return new()
        {
            Id = score.Id,
            HomeTeam = score.HomeTeam,
            AwayTeam = score.AwayTeam,
            HomeScore = score.HomeTeamScore,
            AwayScore = score.AwayTeamScore,
            Added = DateTime.UtcNow
        };
    }

    public static MatchScore ToMatchScore(MatchScoreModel model)
        => new(model.Id, model.HomeTeam!, model.AwayTeam!, model.HomeScore, model.AwayScore);
}