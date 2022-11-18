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

namespace nanoFrameworkDeployer
{
    /// <summary>
    /// This is the main program
    /// </summary>
    internal class Program
    {
        private static CommandlineOptions _options;
        private static MessageHelper _message;
        private static int _returnvalue;
        private static NanoDeviceBase _device;
        private static PortBase _serialDebugClient;

        /// <summary>
        /// Main entry point
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal static int Main(string[] args)
        {
            try
            {
                Parser.Default.ParseArguments<CommandlineOptions>(args)
                                   .WithParsed<CommandlineOptions>(RunLogic)
                                   .WithNotParsed(HandleErrors);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: Parsing arguments threw an exception with message `{ex.Message}`");
                _returnvalue = 1;
            }

            Console.WriteLine($"Exit with return code {_returnvalue}");

            // Force clean
            _serialDebugClient?.StopDeviceWatchers();
            _device?.Disconnect(true);
            _device = null;

            return _returnvalue;
        }

        /// <summary>
        /// On parameter errors, we set the returnvalue to 1 to indicated an error.
        /// </summary>
        /// <param name="errors">List or errors (ignored).</param>
        private static void HandleErrors(IEnumerable<Error> errors)
        {
            _returnvalue = 1;
        }

        private static void RunLogic(CommandlineOptions o)
        {
            int numberOfRetries;
            _options = o;
            _message = new MessageHelper(_options);
            string[] peFiles;
            string workingDirectory;
            List<string> excludedPorts = null;

            // Let's first validate that the directory exist and contains PE files
            _options.PeDirectory = $"{_options.PeDirectory}{Path.DirectorySeparatorChar}";
            if (Directory.Exists(_options.PeDirectory))
            {
                workingDirectory = Path.GetDirectoryName(Path.Combine(_options.PeDirectory));
                peFiles = Directory.GetFiles(workingDirectory, "*.pe");
                if (peFiles.Length == 0)
                {
                    _message.Error("The target directory does not contain any PE file.");
                    _returnvalue = 1;
                    return;
                }

                if(_options.BinaryFileOnly)
                {
                    List<byte[]> assFiles = CreateBinDeploymentFile(peFiles);
                    FileStream deploymentFile = File.Create(Path.Combine(workingDirectory, "deploy.bin"));
                    foreach (var assFile in assFiles)
                    {
                        deploymentFile.Write(assFile.ToArray(), 0, assFile.Length);
                    }

                    deploymentFile.Close();
                    deploymentFile.Dispose();
                    return;
                }
            }
            else
            {
                _message.Error("The target directory is not a valid one.");
                _returnvalue = 1;
                return;
            }

            // Now we will check if there is any exclude file, open it and get the ports
            if (!string.IsNullOrEmpty(_options.PortException))
            {
                if (File.Exists(_options.PortException))
                {
                    var ports = File.ReadAllLines(_options.PortException);
                    if (ports.Length > 0)
                    {
                        excludedPorts = new List<string>();
                        excludedPorts.AddRange(ports);
                    }
                }
                else
                {
                    _message.Error("Exclusion file doesn't exist. Continuing as this error is not critical.");
                }
            }

            _message.Verbose("Finding valid ports");

            int retryCount = 0;            
            // Only 3 tries for a specified port
            numberOfRetries = string.IsNullOrEmpty(_options.ComPort) ? 10 : 3;
            _serialDebugClient = PortBase.CreateInstanceForSerial(true, excludedPorts);

        retryConnection:
            while (!_serialDebugClient.IsDevicesEnumerationComplete)
            {
                Thread.Sleep(1);
            }

            _message.Output($"Found: {_serialDebugClient.NanoFrameworkDevices.Count} devices");

            if (_serialDebugClient.NanoFrameworkDevices.Count == 0)
            {
                if (retryCount > numberOfRetries)
                {
                    _message.Error("Error too many retries");
                    _returnvalue = 1;
                    return;
                }
                else
                {
                    retryCount++;
                    _message.Verbose($"Finding devices, attempt {retryCount}");
                    _serialDebugClient.ReScanDevices();
                    goto retryConnection;
                }
            }

            retryCount = 0;
            _device = _serialDebugClient.NanoFrameworkDevices[0];

            // In case we have multiple ones, we will go for the one passed if the argument is valid
            if ((_serialDebugClient.NanoFrameworkDevices.Count > 1) && (!string.IsNullOrEmpty(_options.ComPort)))
            {
                _device = _serialDebugClient.NanoFrameworkDevices.Where(m => m.SerialNumber == _options.ComPort).First();
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

        retryDebug:
            bool connectResult = _device.DebugEngine.Connect(5000, true, true);
            _message.Output($"Device connect result is {connectResult}. Attempt {retryCount}/{numberOfRetries}");

            if (!connectResult)
            {
                if (retryCount < numberOfRetries)
                {
                    // Give it a bit of time
                    Thread.Sleep(100);
                    retryCount++;
                    goto retryDebug;
                }
                else
                {
                    _message.Error("Error too many retries");
                }
            }

            retryCount = 0;

        retryErase:
            // erase the device
            _message.Output($"Erase deployment block storage. Attempt {retryCount}/{numberOfRetries}.");

            var eraseResult = _device.Erase(
                    EraseOptions.Deployment,
                    null,
                    null);

            _message.Verbose($"Erase result is {eraseResult}.");
            if (!eraseResult)
            {
                if (retryCount < numberOfRetries)
                {
                    // Give it a bit of time
                    Thread.Sleep(400);
                    retryCount++;
                    goto retryErase;
                }
                else
                {
                    _message.Error("Couldn't erase. Too many retries");
                }
            }

            _message.Verbose($"Added {peFiles.Length} assemblies to deploy.");

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
                _message.Error("Write failed.");
                _returnvalue = 1;
            }
            else
            {
                _message.Output("Write successful");
            }
        }

        private static List<byte[]> CreateBinDeploymentFile(string[] peFiles)
        {
            // Keep track of total assembly size
            long totalSizeOfAssemblies = 0;
            List<byte[]> assemblies = new List<byte[]>();
            // now we will deploy all system assemblies
            foreach (string peItem in peFiles)
            {
                // append to the deploy blob the assembly
                using (FileStream fs = File.Open(peItem, FileMode.Open, FileAccess.Read))
                {
                    long length = (fs.Length + 3) / 4 * 4;
                    _message.Verbose($"Adding {peItem} v0 ({length} bytes) to deployment bundle");
                    byte[] buffer = new byte[length];

                    fs.Read(buffer, 0, (int)fs.Length);
                    assemblies.Add(buffer);

                    // Increment totalizer
                    totalSizeOfAssemblies += length;
                }
            }

            _message.Output($"Deploying {peFiles.Length:N0} assemblies to device... Total size in bytes is {totalSizeOfAssemblies}.");

            return assemblies;
        }
    }
}
