// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using nanoFramework.Tools.Debugger;
using System.IO;
using nanoFrameworkDeployer.Helpers;
using CommandLine;
using System.Linq;
using System.IO.Abstractions;
using System.Diagnostics;

namespace nanoFrameworkDeployer
{
    /// <summary>
    /// This is the main program class
    /// </summary>
    internal class Program
    {
        const int RETURN_CODE_SUCCESS = 0;
        const int RETURN_CODE_ERROR = 1;

        private static CommandlineOptions _options;
        private static readonly ConsoleOutputHelper _message = new ConsoleOutputHelper();
        private static int _returnvalue = RETURN_CODE_SUCCESS;
        private static NanoDeviceBase _device;
        private static PortBase _serialDebugClient;

        /// <summary>
        /// This allows overriding the default `System.IO` implementation using `System.IO.Abstractions`, 
        /// so we can override it for tests.
        /// </summary>
        /// <remarks>
        /// Usual filesystem operations start with `fileSystem` rather than just calling them directly.
        /// </remarks>
        internal static IFileSystem fileSystem = new FileSystem();


        /// <summary>
        /// Create Program with the given fileSystem implementation.
        /// </summary>
        /// <remarks>
        /// This is required for running mock filesystem tests.
        /// </remarks>
        internal Program(IFileSystem mockFileSystem) //TODO: handle args.
        {
            fileSystem = mockFileSystem;
        }

        /// <summary>
        /// Required for tests
        /// </summary>
        internal Program()
        {
        }

        /// <summary>
        /// Main entry point
        /// </summary>
        /// <param name="args"></param>
        /// <returns>return code</returns>
        internal static int Main(string[] args)
        {

            Parser.Default.ParseArguments<CommandlineOptions>(args)
                .WithParsed(RunOptionLogic)
                .WithNotParsed(HandleParseErrors);

            if ((_options != null && _options.Verbose == true) || Debugger.IsAttached)
            {
                _message.Verbose($"Program exited with return code: {_returnvalue}");

                if (Debugger.IsAttached) // for checking output before console closes
                {
                    Thread.Sleep(5000);
                }
            }

            ObjectDispose();

            return _returnvalue;
        }

        private static void ObjectDispose()
        {
            // Force clean
            // TODO: should be dispose?!
            _serialDebugClient?.StopDeviceWatchers();
            _device?.Disconnect(true);
            _device = null;
        }

        /// <summary>
        /// On parameter errors, we set the returnvalue to 1 to indicated an error.
        /// </summary>
        /// <param name="errors">List or errors (ignored).</param>
        internal static void HandleParseErrors(IEnumerable<Error> errors)
        {
            foreach (var error in errors)
            {
                _message.Error($"Found command parse error: {error}");
            }
            _message.Verbose($"Perhaps provide an argument?!");

            _returnvalue = RETURN_CODE_ERROR;
        }

        internal static void RunOptionLogic(CommandlineOptions opts)
        {
            int numberOfRetries;
            _options = opts;
            string[] peFiles;
            string workingDirectory;
            List<string> excludedPorts = null;

            // Let's first validate that the directory exist and contains PE files
            // We don't need to check if it is null as it is a required option.
            if (!CheckPeDirExists())
            {
                return;
            }

            workingDirectory = fileSystem.Path.GetDirectoryName(fileSystem.Path.Combine(_options.PeDirectory));
            peFiles = fileSystem.Directory.GetFiles(workingDirectory, "*.pe");
            if (peFiles.Length == 0)
            {
                _message.Error("ERROR: The target directory does not contain any PE files.");
                _returnvalue = RETURN_CODE_ERROR;
                return;
            }

            if (_options.BinaryFileOnly)
            {
                List<byte[]> assFiles = CreateBinDeploymentFile(peFiles);
                var deploymentFile = fileSystem.File.Create(fileSystem.Path.Combine(workingDirectory, "deploy.bin"));
                foreach (var assFile in assFiles)
                {
                    deploymentFile.Write(assFile.ToArray(), 0, assFile.Length);
                }

                deploymentFile.Close();
                deploymentFile.Dispose();
                return;
            }

            // Now we will check if there is any exclude file, open it and get the ports
            if (!string.IsNullOrEmpty(_options.PortException))
            {
                if (fileSystem.File.Exists(_options.PortException))
                {
                    AddSerialPortExclusions(ref excludedPorts, _options.PortException);
                }
                else
                {
                    _message.Error("ERROR: Exclusion file doesn't exist. Continuing as this error is not critical.");
                }
            }

            _message.Verbose("Finding valid serial ports");

            int retryCount = 0;
            // Only 3 tries for a specified port
            numberOfRetries = string.IsNullOrEmpty(_options.ComPort) ? 10 : 3;
            _serialDebugClient = PortBase.CreateInstanceForSerial(true, excludedPorts);

            if (!ConnectToDevice(ref retryCount, ref numberOfRetries))
            {
                return;
            }

            retryCount = 0;
            _device = _serialDebugClient.NanoFrameworkDevices[0];

            // In case we have multiple ones, we will go for the one passed if the argument is valid
            if ((_serialDebugClient.NanoFrameworkDevices.Count > 1) && (!string.IsNullOrEmpty(_options.ComPort)))
            {
                _device = _serialDebugClient.NanoFrameworkDevices.First(m => m.SerialNumber == _options.ComPort);
            }
            else
            {
                _device = _serialDebugClient.NanoFrameworkDevices[0];
            }

            _message.Output($"Deploying on {_device.Description}");

            // check if debugger engine exists
            if (_device.DebugEngine == null)
            {
                _device.CreateDebugEngine();
                _message.Verbose($"Debug engine created.");
            }

            ConnectDebugEngine(ref retryCount, ref numberOfRetries);

            retryCount = 0;

            EraseDevice(ref retryCount, ref numberOfRetries);


            _message.Verbose($"Added {peFiles.Length} assemblies to deploy.");

            DeployAssembiliesToDevice(peFiles);
        }

        internal static bool ConnectToDevice(ref int retryCount, ref int numberOfRetries)
        {
            //TODO: use a while loop
        //retryConnection:
            while (!_serialDebugClient.IsDevicesEnumerationComplete)
            {
                Thread.Sleep(1);
            }

            _message.Output($"Found: {_serialDebugClient.NanoFrameworkDevices.Count} devices");

            if (_serialDebugClient.NanoFrameworkDevices.Count == 0)
            {
                if (retryCount > numberOfRetries)
                {
                    _message.Error("ERROR: too many retries");
                    _returnvalue = RETURN_CODE_ERROR;
                    return false;
                }
                else
                {
                    retryCount++;
                    _message.Verbose($"Finding devices, attempt {retryCount}");
                    _serialDebugClient.ReScanDevices();
                    //goto retryConnection;
                    ConnectToDevice(ref retryCount, ref numberOfRetries);
                }
            }
            return true;
        }

        internal static bool EraseDevice(ref int retryCount, ref int numberOfRetries)
        {
            //TODO: use a while loop
        //retryErase:
            // erase the device
            _message.Output($"Erase deployment block storage. Attempt: {retryCount}/{numberOfRetries}.");

            var eraseResult = _device.Erase(
                    EraseOptions.Deployment,
                    null,
                    null);

            _message.Verbose($"Erase result is: {eraseResult}.");
            if (!eraseResult)
            {
                if (retryCount < numberOfRetries)
                {
                    // Give it a bit of time
                    Thread.Sleep(400);
                    retryCount++;
                    //goto retryErase;
                    EraseDevice(ref retryCount, ref numberOfRetries);

                }
                else
                {
                    _message.Error("ERROR: Could not erase, too many retries");
                    return false;
                }
            }
            return true;
        }

        internal static bool ConnectDebugEngine(ref int retryCount, ref int numberOfRetries)
        {
            //TODO: use a while loop!
        //retryDebug:
            bool connectResult = _device.DebugEngine.Connect(5000, true, true);
            _message.Output($"Device connection result is: {connectResult}. Attempt {retryCount}/{numberOfRetries}");

            if (!connectResult)
            {
                if (retryCount < numberOfRetries)
                {
                    // Give it a bit of time
                    Thread.Sleep(100);
                    retryCount++;
                    //goto retryDebug;
                    ConnectDebugEngine(ref retryCount, ref numberOfRetries);
                }
                else
                {
                    _message.Error("ERROR: too many retries");
                    return false;
                }
            }
            return true;
        }

        internal static void AddSerialPortExclusions(ref List<string> excludedPorts, string portExceptionFilePath)
        {
            //TODO: check file exists?!
            //TODO: check file is formatted correctly?!
            var ports = fileSystem.File.ReadAllLines(portExceptionFilePath);
            if (ports.Length > 0)
            {
                excludedPorts = new List<string>();
                excludedPorts.AddRange(ports);
            }
        }

        internal static List<byte[]> CreateBinDeploymentFile(string[] peFiles)
        {
            _message.Verbose("Merging PE assembilies to create single deployment file...");
            // Keep track of total file binary size
            long deploymentFileSizeInBytes = 0;
            List<byte[]> deploymentFileBytes = new List<byte[]>();
            // now we will add all pe files to create a deployable file
            foreach (var peFile in peFiles)
            {
                // append to the pe blob to the deployment bundle
                using (FileSystemStream fs = fileSystem.File.Open(peFile, FileMode.Open, FileAccess.Read))
                {
                    long bytesToRead = (fs.Length + 3) / 4 * 4; // we add 3 bytes, and then make sure it is aligned to 4?! (for alignment)
                    _message.Verbose($"Adding {peFile} v0 ({bytesToRead} bytes) to deployment bundle");
                    byte[] peFileBytes = new byte[bytesToRead];

                    fs.Read(peFileBytes, 0, (int)fs.Length);
                    deploymentFileBytes.Add(peFileBytes);

                    // Increment totalizer
                    deploymentFileSizeInBytes += bytesToRead;
                }
            }

            _message.Output($"Merged {peFiles.Length:N0} assemblies for deployment... Total size in bytes is {deploymentFileSizeInBytes}.");

            return deploymentFileBytes;
        }

        internal static bool CheckPeDirExists()
        {
            //TODO: a good reason for tests on multiple platforms...
            // add end char for linux folder?!
            _options.PeDirectory = $"{_options.PeDirectory}{fileSystem.Path.DirectorySeparatorChar}";
            if (fileSystem.Directory.Exists(_options.PeDirectory))
            {
                return true;
            }
            else
            {
                _message.Error("ERROR: The target directory does not exist.");
                _returnvalue = RETURN_CODE_ERROR;
                return false;
            }
        }

        internal static void DeployAssembiliesToDevice(string[] peFiles)
        {
            List<byte[]> assemblies = CreateBinDeploymentFile(peFiles);

            // need to keep a copy of the deployment blob for the second attempt (if needed)
            var assemblyCopy = new List<byte[]>(assemblies);

            var deploymentLogger = new Progress<string>((m) => _message.Output(m));
            // Seems to be needed for slow devices
            Thread.Sleep(200);
            if (!_device.DebugEngine.DeploymentExecute(
                assemblyCopy,
                _options.RebootAfterFlash,
                false,
                null,
                deploymentLogger))
            {
                _message.Error("ERROR: Write failed.");
                _returnvalue = RETURN_CODE_ERROR;
            }
            else
            {
                _message.Output("Write successful");
            }
        }
    }
}
