namespace Trogon.KetupaPredicates.Tests.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;

[TestClass]
public class ExpressionParserTest
{
    private const string simplePredicate = "=, 11, 21";
    private const string complexPredicate = "OR, {=, 11, 21}, {<, 11, 21}";

    [TestMethod]
    public void Test_PredicateExpressionCreate_NotNull()
    {
        // Act
        var parser = new ExpressionParserTest();

        // Assert
        Assert.IsNotNull(parser);
    }

    [TestMethod]
    [TestCategory("Simple predicate")]
    public void Test_GetFirstArgumentSimple_OperatorReturned()
    {
        // Arrange
        var parser = new ExpressionParser();

        // Act
        var argument = parser.GetFirstArgument(simplePredicate);

        // Assert
        Assert.AreEqual("=", argument);
    }

    [TestMethod]
    [TestCategory("Complex predicate")]
    public void Test_GetFirstArgumentComplex_OperatorReturned()
    {
        // Arrange
        var parser = new ExpressionParser();

        // Act
        var argument = parser.GetFirstArgument(complexPredicate);

        // Assert
        Assert.AreEqual("OR", argument);
    }

    [TestMethod]
    [TestCategory("Simple predicate")]
    public void Test_GetNextArgument_Eleven()
    {
        // Arrange
        var parser = new ExpressionParser();

        // Act
        var argument = parser.GetNextArgument(simplePredicate, 2);

        // Assert
        Assert.AreEqual(" 11", argument);
    }

    [TestMethod]
    [TestCategory("Simple predicate")]
    public void Test_GetNextArgument_TwentyOne()
    {
        // Arrange
        var parser = new ExpressionParser();

        // Act
        var argument = parser.GetNextArgument(simplePredicate, 7);

        // Assert
        Assert.AreEqual("21", argument);
    }

    [TestMethod]
    [TestCategory("Simple predicate")]
    public void Test_GetNextArgument_EmptyForIndexOutOfRange()
    {
        // Arrange
        var expression = simplePredicate;
        var parser = new ExpressionParser();

        // Act
        var argument = parser.GetNextArgument(expression, expression.Length + 1);

        // Assert
        Assert.AreEqual(string.Empty, argument);
    }

    [TestMethod]
    [TestCategory("Simple predicate")]
    public void Test_GetNextArgument_AllArguments()
    {
        // Arrange
        var expression = simplePredicate;
        var parser = new ExpressionParser();
        var arguments = new List<string>();

        // Act
        int startIndex = 0;
        arguments.Add(parser.GetFirstArgument(expression));
        startIndex += arguments.Last().Length + 1;

        arguments.Add(parser.GetNextArgument(expression, startIndex));
        startIndex += arguments.Last().Length + 1;

        arguments.Add(parser.GetNextArgument(expression, startIndex));
        startIndex += arguments.Last().Length + 1;

        // Assert
        CollectionAssert.Contains(arguments, "=");
        CollectionAssert.Contains(arguments, " 11");
        CollectionAssert.Contains(arguments, " 21");
        Assert.AreEqual(expression.Length + 1, startIndex);
    }
}
