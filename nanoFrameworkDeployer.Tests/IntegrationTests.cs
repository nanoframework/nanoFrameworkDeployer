// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace nanoFrameworkDeployer.Tests
{
#if DEBUG //TODO: find a better argument... e.g. RunLocalIntegrationTests
    [TestClass]
    public class IntegrationTests
    {
        /// <summary>
        /// Path to the program executable folder
        /// </summary>
        /// <remarks>
        /// Should be something like: "C:\<path>\<SolutionName>\<Project>\bin\<ReleaseType>\<TargetFramework>\".
        /// Replaces the test directory path with the project directory path.
        /// For some reason this does not work correctly with .net6, so uses a replace as well.
        /// </remarks>
        private static readonly string programPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace("nanoFrameworkDeployer\\nanoFrameworkDeployer.Tests\\", "nanoFrameworkDeployer\\nanoFrameworkDeployer\\").Replace("net6.0", "net6");

        protected Process StartApplication(string args)
        {

            //Implementation
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = $"{programPath}\\nanoFrameworkDeployer",
                Arguments = args,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };

            return Process.Start(processStartInfo);
        }

        protected Task<string> WaitForResponse(Process process)
        {
            return Task.Run(() =>
            {
                var output = process.StandardOutput.ReadLine();

                return output;
            });
        }

        [TestMethod]
        public void RunApplication_NoArguments_ReturnsError_ShouldRunHelp()
        {
            // Arrange
            var process = StartApplication("");

            // Act
            var outputTask = WaitForResponse(process);
            outputTask.Wait();
            var output = outputTask.Result;

            var expectedOutput = "Found command parse error: CommandLine.MissingRequiredOptionError";

            // Assert
            Assert.AreEqual(expectedOutput, output);
        }

        [TestMethod]
        public void RunApplication_BinaryArgument_NoOtherArgProvided_ReturnsError()
        {
            // Arrange
            var process = StartApplication("-b");

            // Act
            var outputTask = WaitForResponse(process);
            outputTask.Wait();
            var output = outputTask.Result;

            var expectedOutput = "Found command parse error: CommandLine.MissingRequiredOptionError";

            // Assert
            Assert.AreEqual(expectedOutput, output);
        }

        [TestMethod]
        public void RunApplication_VerboseArgument_NoOtherArgProvided_ReturnsError()
        {
            // Arrange
            var process = StartApplication("-v");

            // Act
            var outputTask = WaitForResponse(process);
            outputTask.Wait();
            var output = outputTask.Result;

            var expectedOutput = "Found command parse error: CommandLine.MissingRequiredOptionError";

            // Assert
            Assert.AreEqual(expectedOutput, output);
        }

        //TODO: we should be showing a help output rather than this:
        [TestMethod]
        public void RunApplication_HelpArgument_NoOtherArgProvided_ReturnsError()
        {
            // Arrange
            var process = StartApplication("--help");

            // Act
            var outputTask = WaitForResponse(process);
            outputTask.Wait();
            var output = outputTask.Result;

            var expectedOutput = "Found command parse error: CommandLine.HelpRequestedError";

            // Assert
            Assert.AreEqual(expectedOutput, output);
        }

        // These cannot return sucessfully unless there is something to deploy to.
        // TODO: Think about a simulator, or fakes...
        [TestMethod]
        public void RunApplication_PeDirArgument_NoOtherArgProvided_ReturnsSuccess()
        {
            // Arrange
            var process = StartApplication("-v");

            // Act
            var outputTask = WaitForResponse(process);
            outputTask.Wait();
            var output = outputTask.Result;

            var expectedOutput = "Found command parse error: CommandLine.MissingRequiredOptionError";

            // Assert
            Assert.AreEqual(expectedOutput, output);
        }
#endif

    }
}
