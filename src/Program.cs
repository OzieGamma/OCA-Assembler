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
            if (args.Length == 0 || args[0] == "/help" || args.Length != 2)
            {
                Console.WriteLine("Usage: assembler.exe <in.asm> <out.hex>");
            }
            else if (!File.Exists(args[0]))
            {
                Console.WriteLine("Input file {0} does not exist", args[0]);
            }
            else
            {
                try
                {
                    string name = Path.GetDirectoryName(args[1]);
                    if (name == null)
                    {
                        throw new IOException();
                    }

                    Directory.CreateDirectory(name);

                    AssembleFile(args[0], args[1]);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not create directory for {0}", args[1]);
                    Console.WriteLine(e.StackTrace);
                }
            }

            Console.WriteLine("{0}Done ...", Environment.NewLine);
            Console.Read();
        }

        /// <summary>
        /// Assembles a file and stores it's result in another file.
        /// </summary>
        /// <param name="inputFile">
        /// The input file.
        /// </param>
        /// <param name="outputFile">
        /// The output file.
        /// </param>
        private static void AssembleFile(string inputFile, string outputFile)
        {
            string[] input = File.ReadAllLines(inputFile);

            var withoutComments = RemoveCommentsAndTrim(input).ToFSharpList();
            var result = SourceModule.FromFriendly(withoutComments);

            if (result.IsOk)
            {
                var output = ((Attempt<FSharpList<AsmInstr>>.Ok)result).Item;

                foreach (var instr in output)
                {
                    Console.WriteLine(instr);   
                }
            }
            else
            {
                var errors = ((Attempt<FSharpList<AsmInstr>>.Fail)result).Item.ToList();

                Console.WriteLine("{0} Errors !!! {1}{1}", errors.Count, Environment.NewLine);

                foreach (var error in errors)
                {
                    Console.WriteLine(error);
                }
            }
        }

        /// <summary>
        /// Removes comments. (And trims)
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The input without comments and trimmed.
        /// </returns>
        private static IEnumerable<string> RemoveCommentsAndTrim(IEnumerable<string> input)
        {
            return input.AsParallel().Select(_ => new string(_.Trim().TakeWhile(c => c != '#').ToArray())).Where(_ => _ != string.Empty);
        }

        /// <summary>
        /// Transforms an enumerable to an F# list.
        /// </summary>
        /// <param name="enumerable">
        /// The enumerable.
        /// </param>
        /// <typeparam name="T">
        /// The type of the enumerable.
        /// </typeparam>
        /// <returns>
        /// The F# list.
        /// </returns>
        private static FSharpList<T> ToFSharpList<T>(this IEnumerable<T> enumerable)
        {
            return SeqModule.ToList(enumerable);
        }
    }
}