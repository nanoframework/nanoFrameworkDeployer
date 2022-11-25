using nanoFrameworkDeployer.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System;

namespace nanoFrameworkDeployer.Tests
{
    [TestClass]
    public class ConsoleOutputHelperTests
    {
        [TestMethod]
        public void CheckOutputMessageValid()
        {
            // Arrange
            var writer = new StringWriter();
            Console.SetOut(writer);

            // Act
            var testOutputStr = "Hello Output";
            ConsoleOutputHelper message = new ConsoleOutputHelper();
            message.Output(testOutputStr);

            // Assert
            var sb = writer.GetStringBuilder();
            Assert.AreEqual("Hello Output", sb.ToString().Trim());

        }

        [TestMethod]
        public void CheckVerboseMessageValid()
        {
            // Arrange
            var writer = new StringWriter();
            Console.SetOut(writer);

            // Act
            var testOutputStr = "Hello Output";
            ConsoleOutputHelper message = new ConsoleOutputHelper();
            message.Verbose(testOutputStr);

            // Assert
            var sb = writer.GetStringBuilder();
            Assert.AreEqual("Hello Output", sb.ToString().Trim());
        }

        [TestMethod]
        public void CheckWarningMessageValid()
        {
            // Arrange
            var writer = new StringWriter();
            Console.SetOut(writer);

            // Act
            var testOutputStr = "Hello Output";
            ConsoleOutputHelper message = new ConsoleOutputHelper();
            message.Warning(testOutputStr);

            // Assert
            var sb = writer.GetStringBuilder();
            Assert.AreEqual("Hello Output", sb.ToString().Trim());
        }

        [TestMethod]
        public void CheckErrorMessageValid()
        {
            // Arrange
            var writer = new StringWriter();
            Console.SetOut(writer);

            // Act
            var testOutputStr = "Hello Output";
            ConsoleOutputHelper message = new ConsoleOutputHelper();
            message.Error(testOutputStr);

            // Assert
            var sb = writer.GetStringBuilder();
            Assert.AreEqual("Hello Output", sb.ToString().Trim());
        }

    }
}