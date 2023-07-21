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
        _matches.GetMatchesOrderedByTotalScoreWithRecentFirst();

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
