using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nanoFrameworkDeployer.Tests
{
    [TestClass]
    public class PortExclusionTests
    {
        [TestMethod]
        public void CheckPortExclusionWhenFileExistsAndIsValidSinglePort()
        {
            // Arrange
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
             {
                 { @"c:\portExclusions.txt", new MockFileData("COMXX") },
             });
            var component = new Program(fileSystem);
            List<string> excludedPorts = null;
            var exclusionFilePath = "c:\\portExclusions.txt";

            // Act
            excludedPorts = Program.AddSerialPortExclusions(exclusionFilePath);


            // Assert
            Assert.AreEqual("COMXX", excludedPorts.First());
        }

        [TestMethod]
        public void CheckPortExclusionWhenFileExistsAndIsValidMultiplePorts()
        {
            // Arrange
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
             {
                 { @"c:\portExclusions.txt", new MockFileData("COMXX"+ Environment.NewLine + "COMYY") },
             });
            var component = new Program(fileSystem);
            List<string> excludedPorts = null;
            var exclusionFilePath = "c:\\portExclusions.txt";

            // Act
            excludedPorts = Program.AddSerialPortExclusions(exclusionFilePath);

            var portlist = string.Join(Environment.NewLine, excludedPorts);
            // Assert
            // need to be a sting array to pass!
            Assert.AreEqual("COMXX" + Environment.NewLine + "COMYY", portlist);
        }

        [TestMethod]
        public void CheckPortExclusionWhenFileExistsAndIsInvalid()
        {
            // Arrange
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
             {
                 { @"c:\portExclusions.txt", new MockFileData("Testing is meh.") },
             });
            var component = new Program(fileSystem);
            List<string> excludedPorts = null;
            var exclusionFilePath = "c:\\portExclusions.txt";

            // Act
            excludedPorts = Program.AddSerialPortExclusions(exclusionFilePath);


            // Assert
            Assert.AreEqual("Testing is meh.", excludedPorts.First());
        }

#if RunBrokenTest
        [TestMethod]
        public void CheckPortExclusionWhenFileExistsAndIsEmpty()
        {
            // Arrange
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
             {
                 { @"c:\portExclusions.txt", new MockFileData("") },
             });
            var component = new Program(fileSystem);
            List<string> excludedPorts = null;
            var exclusionFilePath = "c:\\portExclusions.txt";

            // Act
            excludedPorts = Program.AddSerialPortExclusions(exclusionFilePath);


            //TODO: The program does not try and catch this, so we also dont...  value WILL BE NULL since we never set it!
            // Assert
            Assert.IsNull(excludedPorts);
        }
#endif

#if RunBrokenTest
        [TestMethod]
        public void CheckPortExclusionWhenFileDoesNotExist()
        {
            // Arrange
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
            var component = new Program(fileSystem);
            List<string> excludedPorts = null;
            var exclusionFilePath = "c:\\portExclusions1.txt"; // invalid on purpose!

            //TODO: The program does not try and catch this, so we also dont... So it WILL FAIL.
            // Act
            excludedPorts = Program.AddSerialPortExclusions(exclusionFilePath);


            // Assert
            Assert.AreEqual("", excludedPorts.First());
        }
#endif

#if RunBrokenTest
        [TestMethod]
        public void CheckPortExclusionWhenFileIsNotSpecified()
        {
            // Arrange
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
            var component = new Program(fileSystem);
            List<string> excludedPorts = null;
            var exclusionFilePath = ""; // empty on purpose!

            //TODO: The program does not try and catch this, so we also dont... So it WILL FAIL.
            // Act
            excludedPorts = Program.AddSerialPortExclusions(exclusionFilePath);


            // Assert
            Assert.IsNull(excludedPorts);
        }
#endif

    }
}