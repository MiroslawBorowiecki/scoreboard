namespace Scoreboard;

public class Scoreboard
{
    public void StartNewMatch(string homeTeam, string awayTeam)
    {
        if (string.IsNullOrWhiteSpace(homeTeam)) throw new ArgumentException(nameof(homeTeam));
        if (string.IsNullOrWhiteSpace(awayTeam)) throw new ArgumentException(nameof(awayTeam));
    }
}