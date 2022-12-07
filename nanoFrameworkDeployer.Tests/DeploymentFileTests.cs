// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//helpful resource
//https://github.com/reductech/TesseractConnector/blob/60203bc75b1b4f194ff6295d2ecaca6e287deeb4/Tesseract.Tests/TesseractOCRTests.cs

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;

namespace nanoFrameworkDeployer.Tests
{
    [TestClass]
    public class DeploymentFileTests
    {
        [TestMethod]
        public void CheckDirectoryContainsPeFiles()
        {
            // Arrange
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
             {
                 { @"c:\pedir\mypefile.pe", new MockFileData("Testing is meh.") },
             });
            _ = new Program(fileSystem);
           
            // Act
            var result = Program.DirectoryIsValid("c:\\pedir\\");


            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void PeFilesDirectoryContainsInvalidPeFiles()
        {
            // Arrange
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
             {
                 { @"c:\pedir\mypefile.pe", new MockFileData("Testing is meh.") },
             });
            _ = new Program(fileSystem);

            // Act
            var result = Program.DirectoryIsValid("c:\\pedir\\");


            // Assert
            Assert.IsTrue(result);
        }

        // TODO: we are only checking one of many scenarios here, add some more, like invalid paths and alignment etc.
        [TestMethod]
        public void CheckCreateDeploymentBlob()
        {
            // Arrange
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
             {
                 { @"c:\pedir\mypefile.pe", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) },
                 { @"c:\pedir\mypefile2.pe", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
             });
            var expectedResult = new List<byte[]>
            {
                new byte[] { 0x12, 0x34, 0x56, 0xd2 },
                new byte[] { 0x12, 0x34, 0x56, 0xd2 }
            };
            _ = new Program(fileSystem);

            // Act
            var result = Program.CreateBinDeploymentBlob(new string[] { "c:\\pedir\\mypefile.pe", "c:\\pedir\\mypefile2.pe" });


            // Assert
            //TODO: could be Sequence Equal: https://stackoverflow.com/questions/3232744/easiest-way-to-compare-arrays-in-c-sharp
            CollectionAssert.AreEquivalent(expectedResult[0], result[0]);
            CollectionAssert.AreEquivalent(expectedResult[1], result[1]);
            Assert.AreEqual(expectedResult.Count, result.Count);
        }

        [TestMethod]
        public void CheckCreateDeploymentBlob_unaligned()
        {
            // Arrange
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
             {
                 { @"c:\pedir\mypefile.pe", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) },
                 { @"c:\pedir\mypefile2.pe", new MockFileData(new byte[] { 0x12, 0x34, 0x56 }) }
             });
            var expectedResult = new List<byte[]>
            {
                new byte[] { 0x12, 0x34, 0x56, 0xd2 },
                new byte[] { 0x12, 0x34, 0x56, 0x00 }
            };
            _ = new Program(fileSystem);

            // Act
            var result = Program.CreateBinDeploymentBlob(new string[] { "c:\\pedir\\mypefile.pe", "c:\\pedir\\mypefile2.pe" });


            // Assert
            //TODO: could be Sequence Equal: https://stackoverflow.com/questions/3232744/easiest-way-to-compare-arrays-in-c-sharp
            CollectionAssert.AreEquivalent(expectedResult[0], result[0]);
            CollectionAssert.AreEquivalent(expectedResult[1], result[1]);
            Assert.AreEqual(expectedResult.Count, result.Count);
        }

    }
}