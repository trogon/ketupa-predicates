namespace Trogon.KetupaPredicates.Tests.MSTest
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using System.Collections;
    using System.Collections.Generic;

    [TestClass]
    public class PredicateExpressionTest
    {
        private const string simplePredicate = "=, 11, 21";
        private const string complexPredicate = "OR, {=, 13, 31}, {<, 15, 51}";

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
        public void Test_PrepareArguments_IsPrepared()
        {
            // Arrange
            var engine = new PredicateExpression(simplePredicate);

            // Act
            engine.PrepareArguments();

            // Assert
            Assert.IsTrue(engine.IsPrepared);
        }

        [TestMethod]
        [TestCategory("Simple predicate")]
        public void Test_PrepareArguments_OperationExtracted()
        {
            // Arrange
            var engine = new PredicateExpression(simplePredicate);

            // Act
            engine.PrepareArguments();

            // Assert
            Assert.AreEqual("=", engine.Operation);
        }

        [TestMethod]
        [TestCategory("Simple predicate")]
        public void Test_PrepareArguments_ArgumentsExtracted()
        {
            // Arrange
            var engine = new PredicateExpression(simplePredicate);

            // Act
            engine.PrepareArguments();

            // Assert
            CollectionAssert.DoesNotContain(engine.Arguments as ICollection, "=");
            CollectionAssert.Contains(engine.Arguments as ICollection, "11");
            CollectionAssert.Contains(engine.Arguments as ICollection, "21");
        }

        [TestMethod]
        [TestCategory("Simple predicate")]
        public void Test_PrepareArguments_ArgumentsOrederMath()
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
        [DataRow("NOT, True", false, DisplayName = "NOT operation with True value")]
        [DataRow("NOT, False", true, DisplayName = "NOT operation with False value")]
        [DataRow("=, 42, 24", false, DisplayName = "= operation with different values")]
        [DataRow("=, 42, 42", true, DisplayName = "= operation with same values")]
        [DataRow("HasFlag, 181, 8", false, DisplayName = "HasFlag operation that does not has flag")]
        [DataRow("HasFlag, 181, 5", true, DisplayName = "HasFlag operation that has flag")]
        [DataRow("IN, hello, complexText", false, DisplayName = "Text B does not contain A => false")]
        [DataRow("IN, plexT, complexText", true, DisplayName = "Text B contains A => true")]
        [DataRow("IN, complexText, plexT", false, DisplayName = "Text A contains B => false")]
        [DataRow("IN, hello, hello", true, DisplayName = "Text B equals A => true")]
        [DataRow("IN, , hello", true, DisplayName = "Empty text B contains A => true")]
        [DataRow("IN, hello, ", false, DisplayName = "Empty text A contains B => false")]
        [DataRow("IN, , ", true, DisplayName = "Empty text B equals A => true")]
        public void Test_Evaluate(string expression, bool expectedResult)
        {
            // Arrange
            var engine = new PredicateExpression(expression);
            engine.Prepare();

            // Act
            var result = engine.Evaluate();

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        [TestCategory("Simple predicate")]
        [DataRow("=, $var1, 42", "var2", "42", false, DisplayName = "Variable does not exist")]
        [DataRow("=, $var1, 42", "var1", "24", false, DisplayName = "Variable is not equal")]
        [DataRow("=, $var1, 42", "var1", "42", true, DisplayName = "Variable equals")]
        [DataRow("=, $var1, 4\\{\\}$\\,\\\\2", "var1", "4{}$,\\2", true, DisplayName = "Variable equals with token escape")]
        [DataRow("HasFlag, $var1, 42", "var1", null, false, DisplayName = "Variable is null")]
        public void Test_Evaluate_WithVariable(string expression, string varName, string varValue, bool expectedResult)
        {
            // Arrange
            var engine = new PredicateExpression(expression);
            engine.Prepare();

            // Act
            var result = engine.Evaluate(new Dictionary<string, object>
            {
                { varName, varValue }
            });

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        [TestCategory("Simple predicate")]
        [DataRow(true, DisplayName = "Boolean type")]
        [DataRow(135, DisplayName = "Integer type")]
        [DataRow(4.2, DisplayName = "Double type")]
        [DataRow("Example", DisplayName = "String type")]
        public void Test_Evaluate_WithVariable_NonStingType(object varValue)
        {
            // Arrange
            var engine = new PredicateExpression($"=, $var1, {varValue}");
            engine.Prepare();

            // Act
            var result = engine.Evaluate(new Dictionary<string, object>
            {
                { "var1", varValue }
            });

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        [TestCategory("Simple predicate")]
        [DataRow("<, 24, 42", true, DisplayName = "< operation that a < b => true")]
        [DataRow("<, 42, 24", false, DisplayName = "< operation that a > b => false")]
        [DataRow("<, 42, 42", false, DisplayName = "< operation that a = b => false")]
        [DataRow(">, 24, 42", false, DisplayName = "> operation that a < b => false")]
        [DataRow(">, 42, 24", true, DisplayName = "> operation that a > b => true")]
        [DataRow(">, 42, 42", false, DisplayName = "> operation that a = b => false")]
        [DataRow("<=, 24, 42", true, DisplayName = "<= operation that a < b => true")]
        [DataRow("<=, 42, 24", false, DisplayName = "<= operation that a > b => false")]
        [DataRow("<=, 42, 42", true, DisplayName = "<= operation that a = b => true")]
        [DataRow(">=, 24, 42", false, DisplayName = ">= operation that a < b => false")]
        [DataRow(">=, 42, 24", true, DisplayName = ">= operation that a > b => true")]
        [DataRow(">=, 42, 42", true, DisplayName = ">= operation that a = b => true")]
        public void Test_EvaluateCompare(string expression, bool expectedResult)
        {
            // Arrange
            var engine = new PredicateExpression(expression);
            engine.Prepare();

            // Act
            var result = engine.EvaluateCompare(new Dictionary<string, object>());

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        [TestCategory("Simple predicate")]
        [DataRow("AND, False, False", false, DisplayName = "AND that A=false, B=false => false")]
        [DataRow("AND, False, True", false, DisplayName = "AND that A=false, B=true => false")]
        [DataRow("AND, True, False", false, DisplayName = "AND that A=true, B=false => false")]
        [DataRow("AND, True, True", true, DisplayName = "AND that A=true, B=true => true")]
        [DataRow("AND, True, True, True, True", true, DisplayName = "OR that all True => true")]
        [DataRow("AND, True, True, True, False", false, DisplayName = "OR that last one is False => false")]
        public void Test_EvaluateAnd(string expression, bool expectedResult)
        {
            // Arrange
            var engine = new PredicateExpression(expression);
            engine.Prepare();

            // Act
            var result = engine.EvaluateAnd(new Dictionary<string, object>());

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        [TestCategory("Simple predicate")]
        [DataRow("OR, False, False", false, DisplayName = "OR that A=false, B=false => false")]
        [DataRow("OR, False, True", true, DisplayName = "OR that A=false, B=true => true")]
        [DataRow("OR, True, False", true, DisplayName = "OR that A=true, B=false => true")]
        [DataRow("OR, True, True", true, DisplayName = "OR that A=true, B=true => true")]
        [DataRow("OR, False, False, False, False", false, DisplayName = "OR that all False => false")]
        [DataRow("OR, False, False, False, True", true, DisplayName = "OR that last one is True => true")]
        public void Test_EvaluateOr(string expression, bool expectedResult)
        {
            // Arrange
            var engine = new PredicateExpression(expression);
            engine.Prepare();

            // Act
            var result = engine.EvaluateOr(new Dictionary<string, object>());

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        [TestCategory("Simple predicate")]
        [DataRow("HasFlag, 181, 8", false, DisplayName = "HasFlag operation that does not has flag")]
        [DataRow("HasFlag, 181, 5", true, DisplayName = "HasFlag operation that has flag")]
        public void Test_EvaluateHasFlag(string expression, bool expectedResult)
        {
            // Arrange
            var engine = new PredicateExpression(expression);
            engine.Prepare();

            // Act
            var result = engine.EvaluateHasFlag(new Dictionary<string, object>());

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        [TestCategory("Simple predicate")]
        [DataRow("HasFlag, 181, $var1", "8", false, DisplayName = "(String) HasFlag operation that does not has flag")]
        [DataRow("HasFlag, 181, $var1", "5", true, DisplayName = "(String) HasFlag operation that has flag")]
        [DataRow("HasFlag, 181, $var1", 8, false, DisplayName = "(Integer) HasFlag operation that does not has flag")]
        [DataRow("HasFlag, 181, $var1", 5, true, DisplayName = "(Integer) HasFlag operation that has flag")]
        public void Test_EvaluateHasFlag_WithVariable(string expression, object varValue, bool expectedResult)
        {
            // Arrange
            var engine = new PredicateExpression(expression);
            engine.Prepare();

            // Act
            var result = engine.EvaluateHasFlag(new Dictionary<string, object>()
            {
                { "var1", varValue }
            });

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        [TestCategory("Simple predicate")]
        [DataRow("IN, hello, complexText", false, DisplayName = "Text B does not contain A => false")]
        [DataRow("IN, plexT, complexText", true, DisplayName = "Text B contains A => true")]
        [DataRow("IN, complexText, plexT", false, DisplayName = "Text A contains B => false")]
        [DataRow("IN, hello, hello", true, DisplayName = "Text B equals A => true")]
        [DataRow("IN, , hello", true, DisplayName = "Empty text B contains A => true")]
        [DataRow("IN, hello, ", false, DisplayName = "Empty text A contains B => false")]
        [DataRow("IN, , ", true, DisplayName = "Empty text B equals A => true")]
        public void Test_EvaluateIn(string expression, bool expectedResult)
        {
            // Arrange
            var engine = new PredicateExpression(expression);
            engine.Prepare();

            // Act
            var result = engine.EvaluateIn(new Dictionary<string, object>());

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        [TestCategory("Simple predicate")]
        [DataRow("IN, tEa, $var1", true, DisplayName = "Text A is in B => true")]
        [DataRow("IN, 42, $var1", true, DisplayName = "Number A is in B => true")]
        [DataRow("IN, caffe, $var1", false, DisplayName = "Text A is not in B => false")]
        [DataRow("IN, , $var1", true, DisplayName = "Empty text A is in B => true")]
        [DataRow("IN, $var1, tEa", false, DisplayName = "A is an array instead of B => false")]
        public void Test_EvaluateIn_ArrayVariable(string expression, bool expectedResult)
        {
            // Arrange
            var engine = new PredicateExpression(expression);
            engine.Prepare();

            // Act
            var result = engine.EvaluateIn(new Dictionary<string, object>()
            {
                { "var1", new [] { "Hello", "tEa", string.Empty, "42" } }
            });

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        [TestCategory("Complex predicate")]
        public void Test_Prepare_IsPrepared()
        {
            // Arrange
            var engine = new PredicateExpression(complexPredicate);

            // Act
            engine.Prepare();

            // Assert
            Assert.IsTrue(engine.IsPrepared);
        }

        [TestMethod]
        [TestCategory("Complex predicate")]
        public void Test_Prepare_OperationExtracted()
        {
            // Arrange
            var engine = new PredicateExpression(complexPredicate);

            // Act
            engine.Prepare();

            // Assert
            Assert.AreEqual("OR", engine.Operation);
        }

        [TestMethod]
        [TestCategory("Complex predicate")]
        public void Test_Prepare_ArgumentsExtracted()
        {
            // Arrange
            var engine = new PredicateExpression(complexPredicate);

            // Act
            engine.Prepare();

            // Assert
            CollectionAssert.DoesNotContain(engine.Arguments as ICollection, "OR");
            CollectionAssert.Contains(engine.Arguments as ICollection, "{=, 13, 31}");
            CollectionAssert.Contains(engine.Arguments as ICollection, "{<, 15, 51}");
        }

        [TestMethod]
        [TestCategory("Complex predicate")]
        public void Test_Prepare_ArgumentsOrederMath()
        {
            // Arrange
            var engine = new PredicateExpression(complexPredicate);

            // Act
            engine.Prepare();

            // Assert
            CollectionAssert.AreEqual(new[] { "{=, 13, 31}", "{<, 15, 51}" }, engine.Arguments as ICollection);
        }

        [TestMethod]
        [TestCategory("Complex predicate")]
        [DataRow("=,  {=, 11,52 }, {=, 52,11 }", true, DisplayName = "= that A=false, B=false => true")]
        [DataRow("=,  {=, 11,52 }, {=, 42, 42}", false, DisplayName = "= that A=false, B=true => false")]
        [DataRow("=,  {=, 42, 42}, {=, 52,11 }", false, DisplayName = "= that A=true, B=false => false")]
        [DataRow("=,  {=, 42, 42}, {=, 24, 24}", true, DisplayName = "= that A=true, B=true => true")]
        [DataRow("OR, {=, 52,11 }, {=, 52,11 }", false, DisplayName = "OR that A=false, B=false => false")]
        [DataRow("OR, {=, 52,11 }, {=, 24, 24}", true, DisplayName = "OR that A=false, B=true => true")]
        [DataRow("OR, {=, 42, 42}, {=, 52,11 }", true, DisplayName = "OR that A=true, B=false => true")]
        [DataRow("OR, {=, 42, 42}, {=, 24, 24}", true, DisplayName = "OR that A=true, B=true => true")]
        [DataRow("AND,{=, 52,11 }, {=, 52,11 }", false, DisplayName = "AND that A=false, B=false => false")]
        [DataRow("AND,{=, 52,11 }, {=, 24, 24}", false, DisplayName = "AND that A=false, B=true => false")]
        [DataRow("AND,{=, 42, 42}, {=, 52,11 }", false, DisplayName = "AND that A=true, B=false => false")]
        [DataRow("AND,{=, 42, 42}, {=, 24, 24}", true, DisplayName = "AND that A=true, B=true => true")]
        public void Test_Evaluate_Complex(string expression, bool expectedResult)
        {
            // Arrange
            var engine = new PredicateExpression(expression);
            engine.Prepare();

            // Act
            var result = engine.Evaluate();

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        [TestCategory("Complex predicate")]
        [DataRow("=,  {=, $var1, 42}, {=, 24, 24}", "var2", "42", false, DisplayName = "Variable does not exist")]
        [DataRow("OR, {=, 42, $var1}, {=, 52,11 }", "var1", "24", false, DisplayName = "Variable is not equal")]
        [DataRow("AND,{=, $var1, 42}, {=, 24, 24}", "var1", "42", true, DisplayName = "Variable equals")]
        public void Test_Evaluate_ComplexWithVariable(string expression, string varName, string varValue, bool expectedResult)
        {
            // Arrange
            var engine = new PredicateExpression(expression);
            engine.Prepare();

            // Act
            var result = engine.Evaluate(new Dictionary<string, object>
            {
                { varName, varValue }
            });

            // Assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
