namespace Scoreboard.Infrastructure
{
    internal class InMemoryMatchScoreRepository : IMatchScoreRepository
    {
        private readonly Dictionary<Guid, MatchScoreModel> _scores = new();

        public void Add(MatchScoreModel matchScoreModel)
        {
            _scores.Add(matchScoreModel.Id, matchScoreModel);
        }

        public MatchScoreModel? CheckForConflictingMatches(string homeTeam, string awayTeam)
        {
            return _scores.Values.FirstOrDefault(
                ms => ms.HomeTeam.Equals(homeTeam, StringComparison.OrdinalIgnoreCase)
                || ms.HomeTeam.Equals(awayTeam, StringComparison.OrdinalIgnoreCase)
                || ms.AwayTeam.Equals(homeTeam, StringComparison.OrdinalIgnoreCase)
                || ms.AwayTeam.Equals(homeTeam, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<MatchScoreModel> GetMatchesOrderedByTotalScoreWithRecentFirst() =>
            _scores.Values.OrderByDescending(s => s.HomeScore + s.AwayScore)
            .ThenByDescending(s => s.Added);

        public bool Remove(Guid matchScoreId) => _scores.Remove(matchScoreId);

        public bool UpdateScore(Guid matchScoreId, int homeScore, int awayScore)
        {
            if (_scores.TryGetValue(matchScoreId, out MatchScoreModel? model))
            {
                model.HomeScore = homeScore;
                model.AwayScore = awayScore;
                return true;
            }

            return false;
        }
    }
}
