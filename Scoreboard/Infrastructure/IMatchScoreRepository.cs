namespace Scoreboard.Infrastructure;

internal interface IMatchScoreRepository
{
    /// <summary>
    /// Check if either the home team or the away team is already playing a match.
    /// </summary>
    /// <param name="homeTeam">The name of the home team.</param>
    /// <param name="awayTeam">The name of the away team.</param>
    /// <returns>First found conflicting match or null if there are no conflicts.</returns>
    public MatchScoreModel? CheckForConflictingMatches(string homeTeam, string awayTeam);

    /// <summary>
    /// Saves the match.
    /// </summary>
    /// <param name="matchScoreModel">The match to be saved.</param>
    public void Add(MatchScoreModel matchScoreModel);

    /// <summary>
    /// Returns all avaiable matches ordered first by their total score, and then more recent first.
    /// </summary>
    /// <returns>Ordered matches.</returns>
    public IEnumerable<MatchScoreModel> GetMatchesOrderedByTotalScoreWithRecentFirst();
    
    /// <summary>
    /// Removes the match identified by <paramref name="matchScoreId"/>.
    /// </summary>
    /// <param name="matchScoreId">Id of the match to be removed.</param>
    /// <returns>True on success; false on failure (match not found).</returns>
    public bool Remove(Guid matchScoreId);

    /// <summary>
    /// Updates score in the match identified by <paramref name="id"/>>
    /// </summary>
    /// <param name="id">Id of the match to be updated.</param>
    /// <param name="homeScore">Home team's score to set.</param>
    /// <param name="awayScore">Away team's score to set.</param>
    /// <returns>True on success; false on failure (match not found).</returns>
    public bool UpdateScore(Guid id, int homeScore, int awayScore);
}
