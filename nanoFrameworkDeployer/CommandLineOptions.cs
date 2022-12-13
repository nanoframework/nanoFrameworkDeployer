// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommandLine;

namespace nanoFrameworkDeployer
{
    /// <summary>
    /// Class for command line options.
    /// </summary>
    public class CommandlineOptions
    {
        /// <summary>
        /// Gets or sets the folder containing the PE files.
        /// </summary>
        [Option('d', "directory", Required = true, HelpText = "Folder containing the PE files.")]
        public string PeDirectory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether verbose information is shown in the output.
        /// </summary>
        /// <remarks>
        /// The default is false, unless the project is in Debug.
        /// </remarks>
        [Option('v', "verbose", Required = false, HelpText = "Show verbose messages.")]
        public bool Verbose { get; set; }

        /// <summary>
        /// Gets or sets the COM port to use if multiple valid ones are available. By default, the first valid found is used.
        /// </summary>
        [Option('c', "comport", Required = false, HelpText = "The COM port to use if multiple. By default, the first valid found is used.")]
        public string ComPort { get; set; }

        /// <summary>
        /// Gets or sets the COM port exception file. Ports to ignore for the scanning.
        /// </summary>
        /// <remarks>
        /// It is assumed that the file is line delimited.
        /// </remarks>
        [Option('e', "exception", Required = false, HelpText = "COM Port exception file.")]
        public string PortException { get; set; }

        /// <summary>
        /// Gets or sets the rebooting behavior after flash. True to reboot the device.
        /// </summary>
        /// <remarks>
        /// The default is false.
        /// </remarks>
        [Option('r', "reboot", Required = false, HelpText = "Reboot the device after flash.")]
        public bool RebootAfterFlash{ get; set; }

        /// <summary>
        /// Gets or sets the rebooting behavior after flash. True to reboot the device.
        /// </summary>
        /// <remarks>
        /// The default is false.
        /// </remarks>
        [Option('b', "bin", Required = false, HelpText = "Creates a deployment binary file only. This is not attempting to deploy.")]
        public bool BinaryFileOnly { get; set; }
    }
}
