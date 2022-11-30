// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// TODO:  https://pmichaels.net/2022/05/26/unit-testing-a-console-application/
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nanoFrameworkDeployer.Tests
{
    [TestClass]
    public class CommandlineOptionCombinationTests
    {
        [TestMethod]
        public void CheckOptionVerboseValid()
        {
            CommandlineOptions options = new CommandlineOptions
            {
                Verbose = true
            };
            Assert.IsTrue(options.Verbose);
        }

        [TestMethod]
        public void CheckOptionPortValid()
        {
            CommandlineOptions options = new CommandlineOptions
            {
                ComPort = ""
            };
            Assert.AreEqual("", options.ComPort);
        }

        [TestMethod]
        public void CheckPeDirectoryValid()
        {
            CommandlineOptions options = new CommandlineOptions
            {
                PeDirectory = ""
            };
            Assert.AreEqual("", options.PeDirectory);
        }

    }
}