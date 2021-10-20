namespace Trogon.KetupaPredicates.Tests.MSTest
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class ExpressionParserTest
    {
        private const string simplePredicate = "=, 11, 21";
        private const string complexPredicate = "OR, {=, 13, 31}, {<, 15, 51}";

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
        [DataRow('a', Token.None)]
        [DataRow(',', Token.ArgumentSeparator)]
        [DataRow('{', Token.PredicateStart)]
        [DataRow('}', Token.PredicateEnd)]
        public void Test_GetToken_TokenReturned(char symbol, Token expectedToken)
        {
            // Arrange
            var parser = new ExpressionParser();

            // Act
            var token = parser.GetToken(symbol);

            // Assert
            Assert.AreEqual(expectedToken, token);
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

        [TestMethod]
        [TestCategory("Complex predicate")]
        public void TestComplex_GetNextArgument_FirstExpression()
        {
            // Arrange
            var parser = new ExpressionParser();

            // Act
            var argument = parser.GetNextArgument(complexPredicate, 3);

            // Assert
            Assert.AreEqual(" {=, 13, 31}", argument);
        }

        [TestMethod]
        [TestCategory("Complex predicate")]
        public void TestComplex_GetNextArgument_SecondExpression()
        {
            // Arrange
            var parser = new ExpressionParser();

            // Act
            var argument = parser.GetNextArgument(complexPredicate, 13 + 3);

            // Assert
            Assert.AreEqual(" {<, 15, 51}", argument);
        }

        [TestMethod]
        [DataRow("", false)]
        [DataRow(simplePredicate, false)]
        [DataRow(complexPredicate, false)]
        [DataRow("{ =, 11, 21}", true)]
        [DataRow("{OR, {=, 13, 31}, {<, 15, 51} }", true)]
        public void Test_IsExpression(string text, bool expectedResult)
        {
            // Arrange
            var parser = new ExpressionParser();

            // Act
            var result = parser.IsExpression(text);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        [DataRow("", false)]
        [DataRow("$Hello", true)]
        [DataRow("{Hello", false)]
        [DataRow("}Hello", false)]
        [DataRow(",$ello", false)]
        public void Test_IsVariable(string text, bool expectedResult)
        {
            // Arrange
            var parser = new ExpressionParser();

            // Act
            var result = parser.IsVariable(text);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void Test_TrimExpression_BracketsTrimmed()
        {
            // Arrange
            var expression = "EXP { RE } SS{I}ON";
            var parser = new ExpressionParser();

            // Act
            var trimmed = parser.TrimExpression($" {{ {expression}  }}  ");

            // Assert
            Assert.AreEqual(expression, trimmed);
        }
    }
}
