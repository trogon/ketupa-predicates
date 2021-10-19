using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections;

namespace Trogon.KetupaPredicates.Tests.MSTest;

[TestClass]
public class PredicateExpressionTest
{
    private const string simplePredicate = "=, 11, 21";
    private const string complexPredicate = "OR, {=, 11, 21}, {<, 11, 21}";

    [TestMethod]
    [TestCategory("Simple predicate")]
    public void Test_PredicateExpressionCreate_NotNull()
    {
        // Act
        var engine = new PredicateExpression(simplePredicate);

        // Assert
        Assert.IsNotNull(engine);
    }

    [TestMethod]
    [TestCategory("Simple predicate")]
    public void Test_Prepare_IsPrepared()
    {
        // Arrange
        var engine = new PredicateExpression(simplePredicate);

        // Act
        engine.Prepare();

        // Assert
        Assert.AreEqual("=", engine.Operation);
    }

    [TestMethod]
    [TestCategory("Simple predicate")]
    public void Test_Prepare_OperationExtracted()
    {
        // Arrange
        var engine = new PredicateExpression(simplePredicate);

        // Act
        engine.Prepare();

        // Assert
        Assert.AreEqual("=", engine.Operation);
    }

    [TestMethod]
    [TestCategory("Simple predicate")]
    public void Test_Prepare_ArgumentsExtracted()
    {
        // Arrange
        var engine = new PredicateExpression(simplePredicate);

        // Act
        engine.Prepare();

        // Assert
        CollectionAssert.DoesNotContain(engine.Arguments as ICollection, "=");
        CollectionAssert.Contains(engine.Arguments as ICollection, "11");
        CollectionAssert.Contains(engine.Arguments as ICollection, "21");
    }

    [TestMethod]
    [TestCategory("Simple predicate")]
    public void Test_Prepare_ArgumentsOrederMath()
    {
        // Arrange
        var engine = new PredicateExpression(simplePredicate);

        // Act
        engine.Prepare();

        // Assert
        CollectionAssert.AreEqual(new[] { "11", "21" }, engine.Arguments as ICollection);
    }

    [TestMethod]
    [TestCategory("Simple predicate")]
    public void Test_Evaluate_True()
    {
        // Arrange
        var engine = new PredicateExpression("=, 42, 42");
        engine.Prepare();

        // Act
        var result = engine.Evaluate();

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    [TestCategory("Simple predicate")]
    public void Test_Evaluate_False()
    {
        // Arrange
        var engine = new PredicateExpression("=, 42, 24");
        engine.Prepare();

        // Act
        var result = engine.Evaluate();

        // Assert
        Assert.IsFalse(result);
    }
}
