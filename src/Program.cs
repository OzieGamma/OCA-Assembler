// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Oswald Maskens">
//   Copyright 2014 Oswald Maskens
//   
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OCA.Assembler
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Microsoft.FSharp.Collections;

    using OCA.AsmLib;

    /// <summary>
    ///     The program.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The arguments.
        /// </param>
        internal static void Main(string[] args)
        {
            AssemblerOptions options = ProccessArgs(args);

            if (!options.IsValid)
            {
                DisplayUsage();
            }
            else
            {
                try
                {
                    Assembly.Assemble(options);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Something went wrong: {0}", e);
                    Console.WriteLine(e.StackTrace);
                }
            }
        }

        /// <summary>
        ///     The display usage.
        /// </summary>
        private static void DisplayUsage()
        {
            Console.WriteLine("Usage: assembler.exe [in-option] <infile1> [out-option] <outfile>");
            Console.WriteLine("\tIn-Options:");
            Console.WriteLine("\t\t-f --friendly Input is treated as friendly input files");
            Console.WriteLine("\t\t-b --bin Input is treated as binary files");

            Console.WriteLine("\tOut-Options:");
            Console.WriteLine("\t\t-f --friendly Outputs pretty printed text");
            Console.WriteLine("\t\t-b --bin Outputs machine code");
            Console.WriteLine("\t\t-i --intel Outputs machine code in intel hex format");
        }

        /// <summary>
        /// Processes the arguments of the assembler.
        /// </summary>
        /// <param name="args">
        /// The arguments.
        /// </param>
        /// <returns>
        /// The <see cref="AssemblerOptions"/>.
        /// </returns>
        private static AssemblerOptions ProccessArgs(string[] args)
        {
            if (args == null || args.Length != 4)
            {
                return new AssemblerOptions(
                    AssemblerOptions.InputType.Invalid,
                    AssemblerOptions.OutputType.Invalid,
                    null,
                    null);
            }

            return new AssemblerOptions(ProcessInputType(args[0]), ProcessOutputType(args[2]), args[1], args[3]);
        }

        /// <summary>
        /// The process input type.
        /// </summary>
        /// <param name="arg">
        /// The argument where the type should be.
        /// </param>
        /// <returns>
        /// The <see cref="AssemblerOptions.InputType"/>.
        /// </returns>
        private static AssemblerOptions.InputType ProcessInputType(string arg)
        {
            if (arg == "-f" || arg == "--friendly")
            {
                return AssemblerOptions.InputType.Friendly;
            }

            if (arg == "-b" || arg == "--bin")
            {
                return AssemblerOptions.InputType.Bin;
            }

            return AssemblerOptions.InputType.Invalid;
        }

        /// <summary>
        /// The process output type.
        /// </summary>
        /// <param name="arg">
        /// The argument where the type should be..
        /// </param>
        /// <returns>
        /// The <see cref="AssemblerOptions.OutputType"/>.
        /// </returns>
        private static AssemblerOptions.OutputType ProcessOutputType(string arg)
        {
            if (arg == "-f" || arg == "--friendly")
            {
                return AssemblerOptions.OutputType.Friendly;
            }

            if (arg == "-b" || arg == "--bin")
            {
                return AssemblerOptions.OutputType.Bin;
            }

            if (arg == "-i" || arg == "--intel")
            {
                return AssemblerOptions.OutputType.Intel;
            }

            return AssemblerOptions.OutputType.Invalid;
        }
    }
}