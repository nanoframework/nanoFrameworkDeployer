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
            CommandlineOptions options = new CommandlineOptions();
            options.Verbose = true;
            Assert.IsTrue(options.Verbose);
        }

        [TestMethod]
        public void CheckOptionPortValid()
        {
            CommandlineOptions options = new CommandlineOptions();
            options.ComPort = "";
            Assert.AreEqual("", options.ComPort);
        }

        [TestMethod]
        public void CheckPeDirectoryValid()
        {
            CommandlineOptions options = new CommandlineOptions();
            options.PeDirectory = "";
            Assert.AreEqual("", options.PeDirectory);
        }

    }
}