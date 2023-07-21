namespace Scoreboard;

public class Scoreboard
{
    public void StartNewMatch(string homeTeam, string awayTeam)
    public MatchScore StartNewMatch(string homeTeam, string awayTeam)
    {
        if (string.IsNullOrWhiteSpace(homeTeam)) throw new ArgumentException(nameof(homeTeam));
        if (string.IsNullOrWhiteSpace(awayTeam)) throw new ArgumentException(nameof(awayTeam));

        var matchScore = new MatchScore(Guid.NewGuid(), homeTeam, awayTeam, 0, 0);
        return matchScore;
    }
}

public record MatchScore(
    Guid Id,
    string HomeTeam, 
    string AwayTeam, 
    int HomeTeamScore, 
    int AwayTeamScore
    );