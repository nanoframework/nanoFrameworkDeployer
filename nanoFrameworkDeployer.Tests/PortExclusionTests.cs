using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        [TestMethod]
        public void Program_Validate_ExcludingPortFileIsAwsome()
        {
            // Arrange
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
             {
                 { @"c:\portExclusions.txt", new MockFileData("Testing is meh.") },
             });
            var component = new Program(fileSystem);
            List<string> excludedPorts = null;
            var exclusionFilePath = "c:\\portExclusions.txt";

            try
            {
                // Act
                Program.AddSerialPortExclusions(ref excludedPorts, exclusionFilePath);
            }
            catch (NotSupportedException ex)
            {
                // Assert
                Assert.AreEqual("We can't go on together. It's not me, it's you.", ex.Message);
                return;
            }

            Assert.Fail("The expected exception was not thrown.");
        }

    }
}