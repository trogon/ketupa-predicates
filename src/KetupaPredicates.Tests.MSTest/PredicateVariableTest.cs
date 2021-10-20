namespace Trogon.KetupaPredicates.Tests.MSTest
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using System;
    using System.Collections;
    using System.Collections.Generic;

    [TestClass]
    public class PredicateVariableTest
    {
        private const string simpleVariable = "$variable1";
        private const string arrayVariable = "$array1[21][12]";

        [TestMethod]
        [TestCategory("Simple variable")]
        public void Test_PredicateExpressionCreate_NotNull()
        {
            // Act
            var engine = new PredicateVariable(simpleVariable);

            // Assert
            Assert.IsNotNull(engine);
        }

        [TestMethod]
        [TestCategory("Simple variable")]
        [DataRow("$var1", "var2", "42", null, DisplayName = "Variable does not exist")]
        [DataRow("$var1", "var1", "42", "42", DisplayName = "Variable equals")]
        [DataRow("$var1", "var1", null, null, DisplayName = "Variable is null")]
        public void Test_GetValue(string expression, string varName, string varValue, string expectedValue)
        {
            // Arrange
            var engine = new PredicateVariable(expression);
            engine.Prepare();

            // Act
            var value = engine.GetValue(new Dictionary<string, object>
            {
                { varName, varValue }
            });

            // Assert
            Assert.AreEqual(expectedValue, value);
        }

        [TestMethod]
        [TestCategory("Array variable")]
        [DataRow("$var1[0]", "var1", "42", "42", DisplayName = "Index exists")]
        [DataRow("$var1[-1]", "var1", "42", null, DisplayName = "Negative index")]
        [DataRow("$var1[1]", "var1", "42", null, DisplayName = "Index out of range")]
        public void Test_GetValue_FromArray(string expression, string varName, string varValue, string expectedValue)
        {
            // Arrange
            var engine = new PredicateVariable(expression);
            engine.Prepare();

            // Act
            var value = engine.GetValue(new Dictionary<string, object>
            {
                { varName, new [] { varValue } }
            });

            // Assert
            Assert.AreEqual(expectedValue, value);
        }

        [TestMethod]
        [TestCategory("Array variable")]
        [DataRow(true, true, DisplayName = "Boolean type")]
        [DataRow(135, 135, DisplayName = "Integer type")]
        [DataRow(4.2, 4.2, DisplayName = "Double type")]
        [DataRow("Example", "Example", DisplayName = "String type")]
        public void Test_GetValue_FromMixedArray(object varValue, object expectedValue)
        {
            // Arrange
            var engine = new PredicateVariable("$var1[0]");
            engine.Prepare();

            // Act
            var value = engine.GetValue(new Dictionary<string, object>
            {
                { "var1", new [] { varValue } }
            });

            // Assert
            Assert.AreEqual(expectedValue, value);
        }

        [TestMethod]
        [TestCategory("Array variable")]
        public void Test_GetValue_FromArrayMissingIndex()
        {
            // Arrange
            var expectedArray = new[] { "42", "61" };
            var variableDict = new Dictionary<string, object>();
            variableDict["var1"] = expectedArray;
            var engine = new PredicateVariable("$var1");
            engine.Prepare();

            // Act
            var value = engine.GetValue(variableDict);

            // Assert
            CollectionAssert.AreEqual(expectedArray, value as IList);
        }

        [TestMethod]
        [TestCategory("Simple variable")]
        public void Test_Prepare_IsPrepared()
        {
            // Arrange
            var engine = new PredicateVariable(simpleVariable);

            // Act
            engine.Prepare();

            // Assert
            Assert.IsTrue(engine.IsPrepared);
        }

        [TestMethod]
        [TestCategory("Simple variable")]
        public void Test_Prepare_NameExtracted()
        {
            // Arrange
            var engine = new PredicateVariable(simpleVariable);

            // Act
            engine.Prepare();

            // Assert
            Assert.AreEqual("variable1", engine.Name);
        }

        [TestMethod]
        [TestCategory("Simple variable")]
        public void Test_Prepare_NoIndexExtracted()
        {
            // Arrange
            var engine = new PredicateVariable(simpleVariable);

            // Act
            engine.Prepare();

            // Assert
            Assert.IsNull(engine.Indices);
        }

        [TestMethod]
        [TestCategory("Array variable")]
        public void Test_Prepare_ArrayIsPrepared()
        {
            // Arrange
            var engine = new PredicateVariable(arrayVariable);

            // Act
            engine.Prepare();

            // Assert
            Assert.IsTrue(engine.IsPrepared);
        }

        [TestMethod]
        [TestCategory("Array variable")]
        public void Test_Prepare_ArrayNameExtracted()
        {
            // Arrange
            var engine = new PredicateVariable(arrayVariable);

            // Act
            engine.Prepare();

            // Assert
            Assert.AreEqual("array1", engine.Name);
        }

        [TestMethod]
        [TestCategory("Array simple")]
        public void Test_Prepare_IndexExtracted()
        {
            // Arrange
            var engine = new PredicateVariable(arrayVariable);

            // Act
            engine.Prepare();

            // Assert
            Assert.IsNotNull(engine.Indices);
        }

        [TestMethod]
        [TestCategory("Array simple")]
        public void Test_Prepare_IndexOrederMath()
        {
            // Arrange
            var engine = new PredicateVariable(arrayVariable);

            // Act
            engine.Prepare();

            // Assert
            CollectionAssert.AreEqual(new[] { 21, 12 }, engine.Indices as ICollection);
        }

        [TestMethod]
        [TestCategory("Array variable")]
        [DataRow("$var2[a]")]
        [DataRow("$var2[2147483648]")]
        [ExpectedException(typeof(InvalidCastException))]
        public void Test_Prepare_InvalidIndex(string expression)
        {
            // Arrange
            var engine = new PredicateVariable(expression);

            // Act
            engine.Prepare();
        }
    }
}
