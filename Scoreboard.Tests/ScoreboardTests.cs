using Scoreboard.Infrastructure;

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
        Scoreboard scoreboard = SetupScoreboard();

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
        Scoreboard scoreboard = SetupScoreboard();

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
        Scoreboard scoreboard = SetupScoreboard();

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
        Scoreboard scoreboard = SetupScoreboard();
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
        Scoreboard scoreboard = SetupScoreboard();
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
        Scoreboard scoreboard = SetupScoreboard();
        MatchScore matchScore = scoreboard.StartNewMatch("Mexico", "Canada");

        // Act
        void act() => scoreboard.UpdateScore(Guid.Empty, 1, 1);

        // Assert
        var ex = Assert.ThrowsException<ArgumentException>(act);
        StringAssert.Contains(ex.Message, "matchId");
    }

    [TestMethod]
    public void UpdateScore_Throws_WhenEitherScoreIsNegative()
    {
        // Arrange
        Scoreboard scoreboard = SetupScoreboard();
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

    [TestMethod]
    public void UpdateScore_Throws_WhenUnableToFindTheMatch()
    {
        // Arrange
        Scoreboard scoreboard = SetupScoreboard();
        scoreboard.StartNewMatch("Mexico", "Canada");
        Guid invalidGuid = Guid.NewGuid();

        // Act        
        void act() => scoreboard.UpdateScore(invalidGuid, 1, 1);

        // Assert
        var ex = Assert.ThrowsException<ArgumentException>(act);
        var expectedMessage = $"{Scoreboard.MatchNotFoundMessage}{invalidGuid}";
        StringAssert.Contains(ex.Message, expectedMessage);
    }

    [TestMethod]
    public void GetSummary_ReturnsUpdatedScoresSortedByScoreSumAndThenByMostRecent()
    {
        // Arrange
        Scoreboard scoreboard = SetupScoreboard();
        List<MatchScore> originalMatches = StartTestMatches(scoreboard).ToList();
        // Follows the order: Mex - Can, Spa - Bra, Ger - Fra, Uru - Ita, Arg - Aus.
        (int, int)[] updates = new[] { (0, 5), (10, 2), (2, 2), (6, 6), (3, 1) };
        for (int i = 0; i < updates.Length; i++)
        {
            scoreboard.UpdateScore(originalMatches[i].Id, updates[i].Item1, updates[i].Item2);
        }

        // Act
        IEnumerable<MatchScore> results = scoreboard.GetSummary();

        // Assert
        // The assertion below is IMO best compromise between writing the entire table explicitly
        // and a more sophisticated approach that would include some loops and logic.
        var expectedMatches = new MatchScore[]
        {
            SetScore(originalMatches[3], updates[3]), // Uru 6 - Ita 6
            SetScore(originalMatches[1], updates[1]), // Spa 10 - Bra 2
            SetScore(originalMatches[0], updates[0]), // Mex 0 - Can 5
            SetScore(originalMatches[4], updates[4]), // Arg 3 - Aus 1
            SetScore(originalMatches[2], updates[2]) // Ger 2 - Fra 2 
        };
        CollectionAssert.AreEqual(expectedMatches, results.ToList());
    }

    [TestMethod]
    public void FinishMatch_Throws_WhenIdIsEmpty()
    {
        // Arrange
        Scoreboard scoreboard = SetupScoreboard();
        MatchScore matchScore = scoreboard.StartNewMatch("Mexico", "Canada");

        // Act
        void act() => scoreboard.FinishMatch(Guid.Empty);

        // Assert
        var ex = Assert.ThrowsException<ArgumentException>(act);
        StringAssert.Contains(ex.Message, "matchId");
    }

    [TestMethod]
    public void FinishMatch_Throws_WhenUnableToFindTheMatch()
    {
        // Arrange
        Scoreboard scoreboard = SetupScoreboard();
        scoreboard.StartNewMatch("Mexico", "Canada");
        Guid invalidGuid = Guid.NewGuid();

        // Act        
        void act() => scoreboard.FinishMatch(invalidGuid);

        // Assert
        var ex = Assert.ThrowsException<ArgumentException>(act);
        var expectedMessage = $"{Scoreboard.MatchNotFoundMessage}{invalidGuid}";
        StringAssert.Contains(ex.Message, expectedMessage);
    }

    [TestMethod]
    public void GetSummary_DoesNotReturnFinishedMatches()
    {
        // Arrange
        Scoreboard scoreboard = SetupScoreboard();
        StartTestMatches(scoreboard).ToList();
        var sortedScores = scoreboard.GetSummary().ToList();

        // Act
        scoreboard.FinishMatch(sortedScores[2].Id); // Remove in the middle.

        // Assert
        sortedScores.RemoveAt(2);
        var updatedScores = scoreboard.GetSummary().ToList();
        CollectionAssert.AreEqual(sortedScores, updatedScores);

        // Act
        scoreboard.FinishMatch(sortedScores.Last().Id);
        
        // Assert
        sortedScores.RemoveAt(sortedScores.Count - 1);
        updatedScores = scoreboard.GetSummary().ToList();
        CollectionAssert.AreEqual(sortedScores, updatedScores);

        // Act
        scoreboard.FinishMatch(sortedScores[0].Id);

        // Assert
        sortedScores.RemoveAt(0);
        updatedScores = scoreboard.GetSummary().ToList();
        CollectionAssert.AreEqual(sortedScores, updatedScores);
    }

    private static MatchScore SetScore(MatchScore original, (int, int) score)
        => new(original.Id, original.HomeTeam, original.AwayTeam, score.Item1, score.Item2);

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

    private static Scoreboard SetupScoreboard() => new(new InMemoryMatchScoreRepository());
}