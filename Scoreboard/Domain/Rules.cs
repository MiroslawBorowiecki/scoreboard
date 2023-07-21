using Scoreboard.Infrastructure;

namespace Scoreboard.Domain;

internal static class Rules
{
    public static void TeamNameMustContainCharacters(string teamName, string whichTeam)
    {
        if (string.IsNullOrWhiteSpace(teamName))
            throw new ArgumentException(null, whichTeam);
    }

    public static void TeamsCannotBePlayingAnotherMatch(
        IMatchScoreRepository matches, string homeTeam, string awayTeam)
    {
        var conflictingMatch = matches.CheckForConflictingMatches(homeTeam, awayTeam);
        if (conflictingMatch != null) throw new ArgumentException(
            "Cannot start a new match - one of the teams is already playing: " +
            $"{MatchScoreModel.ToMatchScore(conflictingMatch)}");
    }

    public static void MatchIdCannotBeEmtpy(Guid matchId)
    {
        if (matchId == Guid.Empty) throw new ArgumentException(null, nameof(matchId));
    }

    public static void ScoreCannotBeNegative(int homeScore, int awayScore)
    {
        if (homeScore < 0) throw new ArgumentOutOfRangeException(nameof(homeScore));
        if (awayScore < 0) throw new ArgumentOutOfRangeException(nameof(awayScore));
    }
}
