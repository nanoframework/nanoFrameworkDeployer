// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
            _ = new Program(fileSystem);
            var exclusionFilePath = "c:\\portExclusions.txt";

            // Act
            List<string> excludedPorts = Program.AddSerialPortExclusions(exclusionFilePath);


            // Assert
            Assert.AreEqual("COMXX", excludedPorts.First());
            Assert.AreEqual(1, excludedPorts.Count);
        }

        [TestMethod]
        public void CheckPortExclusionWhenFileExistsAndIsValidMultiplePorts_UsingLineDelimter()
        {
            // Arrange
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
             {
                 { @"c:\portExclusions.txt", new MockFileData("COMXX"+ Environment.NewLine + "COMYY") },
             });
            _ = new Program(fileSystem);
            var exclusionFilePath = "c:\\portExclusions.txt";

            // Act
            List<string> excludedPorts = Program.AddSerialPortExclusions(exclusionFilePath);

            var portlist = string.Join(Environment.NewLine, excludedPorts);
            // Assert
            // need to be a sting array to pass!
            Assert.AreEqual("COMXX" + Environment.NewLine + "COMYY", portlist);
            Assert.AreEqual(2, excludedPorts.Count);
        }

        //TODO: requires a fix as the program does not check delimeters!
        //[TestMethod]
        //public void CheckPortExclusionWhenFileExistsAndIsValidMultiplePorts_UsingSemiColonDelimter()
        //{
        //    // Arrange
        //    var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        //     {
        //         { @"c:\portExclusions.txt", new MockFileData("COMXX;COMYY") },
        //     });
        //    _ = new Program(fileSystem);
        //    var exclusionFilePath = "c:\\portExclusions.txt";

        //    // Act
        //    List<string> excludedPorts = Program.AddSerialPortExclusions(exclusionFilePath);

        //    var portlist = string.Join(";", excludedPorts);
        //    // Assert
        //    // need to be a sting array to pass!
        //    Assert.AreEqual($"COMXX;COMYY", portlist);
        //    Assert.AreEqual(2, excludedPorts.Count);
        //}

        [TestMethod]
        public void CheckPortExclusionWhenFileExistsAndIsInvalid()
        {
            // Arrange
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
             {
                 { @"c:\portExclusions.txt", new MockFileData("Testing is meh.") },
             });
            _ = new Program(fileSystem);
            var exclusionFilePath = "c:\\portExclusions.txt";

            // Act
            List<string> excludedPorts = Program.AddSerialPortExclusions(exclusionFilePath);


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
            _ = new Program(fileSystem);
            var exclusionFilePath = "c:\\portExclusions.txt";

            // Act
            List<string> excludedPorts = Program.AddSerialPortExclusions(exclusionFilePath);


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
            _ = new Program(fileSystem);
            var exclusionFilePath = "c:\\portExclusions1.txt"; // invalid on purpose!

            //TODO: The program does not try and catch this, so we also dont... So it WILL FAIL.
            // Act
            List<string> excludedPorts = Program.AddSerialPortExclusions(exclusionFilePath);


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
            _ = new Program(fileSystem);
            var exclusionFilePath = ""; // empty on purpose!

            //TODO: The program does not try and catch this, so we also dont... So it WILL FAIL.
            // Act
            List<string> excludedPorts = Program.AddSerialPortExclusions(exclusionFilePath);


            // Assert
            Assert.IsNull(excludedPorts);
        }
#endif

    }
}