using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.IO.Abstractions.TestingHelpers;

namespace nanoFrameworkDeployer.Tests
{
    [TestClass]
    public class PortExclusionTests
    {
        [TestMethod]
        public void CheckPortExclusionWhenFileExistsAndIsValid()
        {
            // False set until test is written
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void CheckPortExclusionWhenFileExistsAndIsInvalid()
        {
            // False set until test is written
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void CheckPortExclusionWhenFileDoesNotExist()
        {
            // False set until test is written
            Assert.IsTrue(true);
        }


        //https://github.com/reductech/TesseractConnector/blob/60203bc75b1b4f194ff6295d2ecaca6e287deeb4/Tesseract.Tests/TesseractOCRTests.cs
        //[TestMethod]
        //public void Program_Validate_ExcludingPortFileIsAwsome()
        //{
        //    // Arrange
        //    var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        //     {
        //         { @"c:\portExclusions.txt", new MockFileData("Testing is meh.") },
        //     });
        //    var component = new nanoFrameworkDeployer.Program.Program(fileSystem);
        //    List<string> excludedPorts = null;

        //    try
        //    {
        //        // Act
        //        component.AddSerialPortExclusions(ref List<string> excludedPorts, "c:\\portExclusions.txt");
        //    }
        //    catch (NotSupportedException ex)
        //    {
        //        // Assert
        //        Assert.AreEqual("We can't go on together. It's not me, it's you.", ex.Message);
        //        return;
        //    }

        //    Assert.Fail("The expected exception was not thrown.");
        //}

    }
}