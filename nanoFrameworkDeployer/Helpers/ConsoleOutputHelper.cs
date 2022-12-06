﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace nanoFrameworkDeployer.Helpers
{
    /// <summary>
    /// Helper methods to write messages to the console.
    /// </summary>
    public static class ConsoleOutputHelper
    {

        /// <summary>
        /// Helper method for output messages.
        /// </summary>
        /// <param name="message">Message to show in output mode.</param>
        public static void Output(string message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        /// Helper method for verbose messages.
        /// </summary>
        /// <param name="message">Message to show in verbose mode.</param>
        public static void Verbose(string message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(message);
            Console.ResetColor();

        }

        /// <summary>
        /// Helper method for warning messages.
        /// </summary>
        /// <param name="message">Message to show in warning mode.</param>
        public static void Warning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        /// <summary>
        /// Helper method for error messages.
        /// </summary>
        /// <param name="message">Message to show in error mode.</param>
        public static void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
