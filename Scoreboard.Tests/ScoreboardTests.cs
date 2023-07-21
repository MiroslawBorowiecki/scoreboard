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
}