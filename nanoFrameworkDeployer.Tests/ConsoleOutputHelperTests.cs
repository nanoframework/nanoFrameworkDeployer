// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using nanoFrameworkDeployer.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System;

namespace nanoFrameworkDeployer.Tests
{
    [TestClass]
    public class ConsoleOutputHelperTests
    {
        //TODO: to make these tests useful, we need to be able to check font colours.

        [TestMethod]
        public void CheckOutputMessageValid()
        {
            // Arrange
            var writer = new StringWriter();
            Console.SetOut(writer);

            // Act
            var testOutputStr = "Hello Output";
            ConsoleOutputHelper.Output(testOutputStr);

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
            ConsoleOutputHelper.Verbose(testOutputStr);

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
            ConsoleOutputHelper.Warning(testOutputStr);

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
            ConsoleOutputHelper.Error(testOutputStr);

            // Assert
            var sb = writer.GetStringBuilder();
            Assert.AreEqual("Hello Output", sb.ToString().Trim());
        }

    }
}