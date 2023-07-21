using System;

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
        // Arrange
        Scoreboard scoreboard = new();

        // Act
        void act() => scoreboard.StartNewMatch(homeTeam, awayTeam);

        //Assert
        var ex = Assert.ThrowsException<ArgumentException>(act);
        StringAssert.Contains(ex.Message, fieldName);
    }

    [TestMethod]
    public void StartNewMatch_SetsInitialScore0To0()
    {
        // Arrange
        Scoreboard scoreboard = new();

        // Act
        MatchScore matchScore = scoreboard.StartNewMatch("Mexico", "Canada");

        // Assert
        Assert.AreEqual(0, matchScore.HomeTeamScore);
        Assert.AreEqual(0, matchScore.AwayTeamScore);
    }

    [TestMethod]
    public void StartNewMatch_ReturnsAUniqueMatchIdForFutureReference()
    {
        // Arrange
        Scoreboard scoreboard = new();

        // Act
        MatchScore matchScore1 = scoreboard.StartNewMatch("Mexico", "Canada");
        MatchScore matchScore2 = scoreboard.StartNewMatch("Spain", "Brazil");

        // Assert
        Assert.AreNotEqual(Guid.Empty, matchScore1.Id);
        Assert.AreNotEqual(Guid.Empty, matchScore2.Id);
        Assert.AreNotEqual(matchScore1.Id, matchScore2.Id);
    }

    [TestMethod]
    public void StartNewMatch_Throws_WhenOneOfTheTeamsIsAlreadyPlaying()
    {
        // Arrange
        Scoreboard scoreboard = new();
        MatchScore matchScore = scoreboard.StartNewMatch("Mexico", "Canada");

        // Act
        void actHome() => scoreboard.StartNewMatch("Irrelevant", "Mexico");
        void actAway() => scoreboard.StartNewMatch("Canada", "Irrelevant");

        // Assert
        var ex = Assert.ThrowsException<ArgumentException>(actHome);
        StringAssert.Contains(ex.Message, matchScore.ToString());

        ex = Assert.ThrowsException<ArgumentException>(actAway);
        StringAssert.Contains(ex.Message, matchScore.ToString());
    }

    [TestMethod]
    public void GetSummary_ReturnsRecentlyStartedMatchesInReverseOrder()
    {
        // Arrange
        Scoreboard scoreboard = new();
        IEnumerable<MatchScore> originalMatches = StartTestMatches(scoreboard);

        // Act
        IEnumerable<MatchScore> results = scoreboard.GetSummary();

        // Assert
        CollectionAssert.AreEqual(originalMatches.Reverse().ToArray(), results.ToArray());
    }

    [TestMethod]
    public void UpdateScore_Throws_WhenIdIsEmpty()
    {
        // Arrange
        Scoreboard scoreboard = new();
        MatchScore matchScore = scoreboard.StartNewMatch("Mexico", "Canada");

        // Act
        void act() => scoreboard.UpdateScore(Guid.Empty, 1, 1);

        // Assert
        var ex = Assert.ThrowsException<ArgumentException>(act);
        StringAssert.Contains(ex.Message, "id");
    }

    [TestMethod]
    public void UpdateScore_Throws_WhenEitherScoreIsNegative()
    {
        // Arrange
        Scoreboard scoreboard = new();
        MatchScore matchScore = scoreboard.StartNewMatch("Mexico", "Canada");

        // Act
        void actHome() => scoreboard.UpdateScore(matchScore.Id, -1, 1);
        void actAway() => scoreboard.UpdateScore(matchScore.Id, 1, -1);

        // Assert
        var ex = Assert.ThrowsException<ArgumentOutOfRangeException>(actHome);
        StringAssert.Contains(ex.Message, "homeScore");

        ex = Assert.ThrowsException<ArgumentOutOfRangeException>(actAway);
        StringAssert.Contains(ex.Message, "awayScore");
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