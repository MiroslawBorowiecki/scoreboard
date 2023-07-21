namespace Scoreboard;

public class Scoreboard
{
    public const string MatchNotFoundMessageFormat = "Match not found. ID: {0};";
    private readonly List<MatchScoreModel> _scores = new();

    public MatchScore StartNewMatch(string homeTeam, string awayTeam)
    {
        if (string.IsNullOrWhiteSpace(homeTeam)) throw new ArgumentException(null, nameof(homeTeam));
        if (string.IsNullOrWhiteSpace(awayTeam)) throw new ArgumentException(null, nameof(awayTeam));

        var conflictingMatch = _scores.Find(
            ms => ms.HomeTeam.Equals(homeTeam, StringComparison.OrdinalIgnoreCase)
            || ms.HomeTeam.Equals(awayTeam, StringComparison.OrdinalIgnoreCase)
            || ms.AwayTeam.Equals(homeTeam, StringComparison.OrdinalIgnoreCase)
            || ms.AwayTeam.Equals(homeTeam, StringComparison.OrdinalIgnoreCase));
        if (conflictingMatch != null) throw new ArgumentException(
            "Cannot start a new match - one of the teams is already playing: " +
            $"{MatchScoreModel.ToMatchScore(conflictingMatch)}");

        var matchScore = new MatchScore(Guid.NewGuid(), homeTeam, awayTeam, 0, 0);
        _scores.Add(MatchScoreModel.FromMatchScore(matchScore));
        return matchScore;
    }

    public IEnumerable<MatchScore> GetSummary() => _scores
        .OrderByDescending(s => s.HomeScore + s.AwayScore)
        .ThenByDescending(s => s.Added)
        .Select(MatchScoreModel.ToMatchScore);

    public void UpdateScore(Guid id, int homeScore, int awayScore)
    {
        if (id == Guid.Empty) throw new ArgumentException(null, nameof(id));

        if (homeScore < 0) throw new ArgumentOutOfRangeException(nameof(homeScore));
        if (awayScore < 0) throw new ArgumentOutOfRangeException(nameof(awayScore));

        MatchScoreModel? scoreToUpdate = _scores.Find(ms => ms.Id == id);

        if (scoreToUpdate == null) throw new ArgumentException(
            string.Format(MatchNotFoundMessageFormat, id.ToString()));

        scoreToUpdate.HomeScore = homeScore;
        scoreToUpdate.AwayScore = awayScore;
        // Here that's it. With proper persistence it's not that easy...
    }

    public void FinishMatch(Guid id)
    {
        if (id == Guid.Empty) throw new ArgumentException(null, nameof(id));
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