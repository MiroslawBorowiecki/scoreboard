namespace Scoreboard;

public class Scoreboard
{
    private List<MatchScore> _scores = new List<MatchScore>();

    public MatchScore StartNewMatch(string homeTeam, string awayTeam)
    {
        if (string.IsNullOrWhiteSpace(homeTeam)) throw new ArgumentException(nameof(homeTeam));
        if (string.IsNullOrWhiteSpace(awayTeam)) throw new ArgumentException(nameof(awayTeam));

        var conflictingMatch = _scores.Find(
            ms => ms.HomeTeam.Equals(homeTeam, StringComparison.OrdinalIgnoreCase)
            || ms.HomeTeam.Equals(awayTeam, StringComparison.OrdinalIgnoreCase)
            || ms.AwayTeam.Equals(homeTeam, StringComparison.OrdinalIgnoreCase)
            || ms.AwayTeam.Equals(homeTeam, StringComparison.OrdinalIgnoreCase));
        if (conflictingMatch != null) throw new ArgumentException(
            $"Cannot start a new match - one of the teams is already playing: {conflictingMatch}");

        var matchScore = new MatchScore(Guid.NewGuid(), homeTeam, awayTeam, 0, 0);
        _scores.Add(matchScore);
        return matchScore;
    }

    public IEnumerable<MatchScore> GetSummary() => _scores.Reverse<MatchScore>();
}

public record MatchScore(
    Guid Id,
    string HomeTeam, 
    string AwayTeam, 
    int HomeTeamScore, 
    int AwayTeamScore
    );