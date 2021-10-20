namespace Trogon.KetupaPredicates.Tests.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class ParserStateTest
{
    [TestMethod]
    public void Test_ParserStateCreate_NotNull()
    {
        // Act
        var state = new ParserState();

        // Assert
        Assert.IsNotNull(state);
    }

    [TestMethod]
    [DataRow(Token.None, 2)]
    [DataRow(Token.ArgumentSeparator, 2)]
    [DataRow(Token.PredicateStart, 3)]
    [DataRow(Token.PredicateEnd, 1)]
    public void Test_GetBracketLevel_LevelReturned(Token token, int expectedLevel)
    {
        // Arrange
        var state = new ParserState();

        // Act
        var result = state.GetBracketLevel(token, 2);

        // Assert
        Assert.AreEqual(expectedLevel, result);
    }

    [TestMethod]
    [DataRow(Token.None, 0)]
    [DataRow(Token.ArgumentSeparator, 0)]
    [DataRow(Token.PredicateStart, 1)]
    [DataRow(Token.PredicateEnd, -1)]
    public void Test_Update_LevelUpdated(Token token, int expectedLevel)
    {
        // Arrange
        var state = new ParserState();

        // Act
        state.Update(token);

        // Assert
        Assert.AreEqual(expectedLevel, state.BracketLevel);
    }
}
