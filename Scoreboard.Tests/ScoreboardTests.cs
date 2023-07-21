namespace Scoreboard.Tests;

[TestClass]
public class ScoreboardTests
{
    [TestMethod]
    [DataRow("Mexico", null, "awayTeam", DisplayName = "AwayTeamNull")]
    [DataRow("Mexico", "", "awayTeam", DisplayName = "AwayTeamEmpty")]
    [DataRow("Mexico", "\n\r\t ", "awayTeam", DisplayName = "AwayTeamAllWhitespace")]
    [DataRow(null, "Canada", "homeTeam", DisplayName = "HomeTeamNull")]
    [DataRow("", "Canada", "homeTeam", DisplayName = "HomeTeamEmpty")]
    [DataRow("\n\r\t ", "Canada", "homeTeam", DisplayName = "HomeTeamAllWhitespace")]
    public void StartNewMatch_Throws_WhenHomeOrAwayTeamAreNotProvided(
        string homeTeam, string awayTeam, string fieldName)
    {
        Scoreboard scoreboard = new();

        var ex = Assert.ThrowsException<ArgumentException>(
            () => scoreboard.StartNewMatch(homeTeam, awayTeam));
        StringAssert.Contains(ex.Message, fieldName);
    }

    [TestMethod]
    public void StartNewMatch_SetsInitialScore0To0()
    {
        Scoreboard scoreboard = new();

        MatchScore matchScore = scoreboard.StartNewMatch("Mexico", "Canada");

        Assert.AreEqual(0, matchScore.HomeTeamScore);
        Assert.AreEqual(0, matchScore.AwayTeamScore);
    }

    [TestMethod]
    public void StartNewMatch_ReturnsAUniqueMatchIdForFutureReference()
    {
        Scoreboard scoreboard = new();

        MatchScore matchScore1 = scoreboard.StartNewMatch("Mexico", "Canada");
        MatchScore matchScore2 = scoreboard.StartNewMatch("Spain", "Brazil");


        Assert.AreNotEqual(Guid.Empty, matchScore1.Id);
        Assert.AreNotEqual(Guid.Empty, matchScore2.Id);
        Assert.AreNotEqual(matchScore1.Id, matchScore2.Id);
    }

    [TestMethod]
    public void GetSummary_ReturnsRecentlyStartedMatchesInReverseOrder()
    {
        Scoreboard scoreboard = new();
        IEnumerable<MatchScore> originalMatches = StartTestMatches(scoreboard);

        IEnumerable<MatchScore> results = scoreboard.GetSummary();

        CollectionAssert.AreEqual(originalMatches.Reverse().ToArray(), results.ToArray());
    }

    private static IEnumerable<MatchScore> StartTestMatches(Scoreboard scoreboard)
    {
        return new[]
        {
            scoreboard.StartNewMatch("Mexico", "Canada"),
            scoreboard.StartNewMatch("Spain", "Brazil"),
            scoreboard.StartNewMatch("Germany", "France"),
            scoreboard.StartNewMatch("Uruguay", "Italy"),
            scoreboard.StartNewMatch("Argentina", "Australia")
        };
    }
}