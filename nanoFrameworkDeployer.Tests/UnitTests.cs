//helpful resource
//https://github.com/reductech/TesseractConnector/blob/60203bc75b1b4f194ff6295d2ecaca6e287deeb4/Tesseract.Tests/TesseractOCRTests.cs

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;

namespace nanoFrameworkDeployer.Tests
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void CheckDirectoryContainsPeFiles()
        {
            // Arrange
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
             {
                 { @"c:\pedir\mypefile.pe", new MockFileData("Testing is meh.") },
             });
            var component = new Program(fileSystem);
           
            // Act
            var result = Program.DirectoryIsValid("c:\\pedir\\");


            // Assert
            Assert.IsTrue(result);
        }

        // TODO: we are only checking one of many scenarios here, add some more, like invalid paths and alignment etc.
        // But need to get this one working first!
#if RunBrokenTest
        [TestMethod]
        public void CheckCreateDeploymentBlob()
        {
            // Arrange
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
             {
                 { @"c:\pedir\mypefile.pe", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) },
                 { @"c:\pedir\mypefile2.pe", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
             });
            var expectedResult = new List<byte[]>(); //{ //TODO: why List<byte[][] >
            //    new byte[] { 0x12, 0x34, 0x56, 0xd2 }, // for some reason, this has a null 
            //    new byte[] { 0x12, 0x34, 0x56, 0xd2 }  // for some reason, this has a null 
            //};
            expectedResult.Add(new byte[] { 0x12, 0x34, 0x56, 0xd2 });
            expectedResult.Add(new byte[] { 0x12, 0x34, 0x56, 0xd2 });
            var component = new Program(fileSystem);

            // Act
            var result = Program.CreateBinDeploymentBlob(new string[] { "c:\\pedir\\mypefile.pe", "c:\\pedir\\mypefile2.pe" });


            // Assert
            //TODO: should be AreEqual
            CollectionAssert.AreEquivalent(expectedResult, result);
        }
#endif

    }
}