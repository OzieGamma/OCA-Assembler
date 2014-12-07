// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Assembly.cs" company="Oswald Maskens">
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
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using Microsoft.FSharp.Collections;

    using OCA.AsmLib;

    /// <summary>
    ///     The assembly.
    /// </summary>
    internal static class Assembly
    {
        /// <summary>
        /// The assemble.
        /// </summary>
        /// <param name="options">
        /// The options.
        /// </param>
        public static void Assemble(AssemblerOptions options)
        {
            Time("From", () => From(options.InType, options.InputFile))
                    .TryContinue("from", instructions => To(instructions, options.OutType, options.OutputFile));
        }

        /// <summary>
        /// Prints errors if the attempt failed, otherwise continues computation.
        /// </summary>
        /// <param name="attempt">
        /// The attempt.
        /// </param>
        /// <param name="phase">
        /// The phase.
        /// </param>
        /// <param name="f">
        /// The function to continue with.
        /// </param>
        /// <typeparam name="T">
        /// The type of the attempt.
        /// </typeparam>
        private static void TryContinue<T>(
            this GenericAttempt<T, Positioned<string>> attempt, 
            string phase, 
            Action<T> f)
        {
            if (attempt.IsOk)
            {
                f(((GenericAttempt<T, Positioned<string>>.Ok)attempt).Item);
            }
            else
            {
                List<Positioned<string>> errors = ((GenericAttempt<T, Positioned<string>>.Fail)attempt).Item.ToList();
                List<string> formatted = errors.Select(SourceModule.FormatPositionInError).ToList();

                Console.WriteLine("{0} Errors during {1} phase !!! {2}{2}", formatted.Count, phase, Environment.NewLine);

                foreach (string error in formatted)
                {
                    Console.WriteLine(error);
                }
            }
        }

        /// <summary>
        /// The to.
        /// </summary>
        /// <param name="instructions">
        /// The instructions.
        /// </param>
        /// <param name="outputType">
        /// The output type.
        /// </param>
        /// <param name="outputFile">
        /// The output file.
        /// </param>
        private static void To(
            IEnumerable<Positioned<AsmInstr>> instructions, 
            AssemblerOptions.OutputType outputType, 
            string outputFile)
        {
            switch (outputType)
            {
                case AssemblerOptions.OutputType.Friendly:
                    Time("to", () => AssemblyModule.ToFriendly(instructions)).TryContinue(
                        "to", 
                        output =>
                            {
                                IEnumerable<string> strings = output.Select(PositionedModule.RemovePosition);
                                File.WriteAllLines(outputFile, strings);
                            });
                    break;
                case AssemblerOptions.OutputType.Bin:
                    Time("to", () => AssemblyModule.ToBin(instructions)).TryContinue(
                        "to", 
                        output =>
                            {
                                IEnumerable<uint> notPositioned = output.Select(PositionedModule.RemovePosition);
                                IEnumerable<byte> bytes = notPositioned.SelectMany(BitConverter.GetBytes);
                                File.WriteAllBytes(outputFile, bytes.ToArray());
                            });
                    break;
                case AssemblerOptions.OutputType.Intel:
                    Time("to", () => AssemblyModule.ToIntelHex(instructions)).TryContinue(
                        "to", 
                        output =>
                            {
                                IEnumerable<string> strings = output.Select(PositionedModule.RemovePosition);
                                File.WriteAllLines(outputFile, strings);
                            });
                    break;
                default:
                    throw new ArgumentException("Unknown output type" + outputType);
            }
        }

        /// <summary>
        /// The from.
        /// </summary>
        /// <param name="inputType">
        /// The input type.
        /// </param>
        /// <param name="inputFile">
        /// The input file.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        private static GenericAttempt<FSharpList<Positioned<AsmInstr>>, Positioned<string>> From(
            AssemblerOptions.InputType inputType, 
            string inputFile)
        {
            switch (inputType)
            {
                case AssemblerOptions.InputType.Friendly:
                    return
                        AssemblyModule.FromFriendly(
                            SourceModule.PositionRemoveCommentsAndTokenize(inputFile, File.ReadAllText(inputFile)));

                case AssemblerOptions.InputType.Bin:
                    byte[] bytes = File.ReadAllBytes(inputFile);
                    if (bytes.Length % 4 != 0)
                    {
                        string msg = "Invalid binary file: " + inputType;
                        var error = new Positioned<string>(msg, new Position(0, 0, inputFile));
                        var errors = new FSharpList<Positioned<string>>(error, FSharpList<Positioned<string>>.Empty);
                        return GenericAttempt<FSharpList<Positioned<AsmInstr>>, Positioned<string>>.NewFail(errors);
                    }

                    var words = new List<uint>();
                    for (int i = 0; i < bytes.Length; i += 4)
                    {
                        words.Add(BitConverter.ToUInt32(bytes, i));
                    }

                    return
                        AssemblyModule.FromBin(
                            words.Select((x, index) => new Positioned<uint>(x, new Position(0, (uint)index, inputFile))));

                default:
                    throw new ArgumentException("Unknown input type" + inputType);
            }
        }

        /// <summary>
        /// Times a function.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="f">
        /// The function.
        /// </param>
        /// <typeparam name="T">
        /// The return type of f.
        /// </typeparam>
        /// <returns>
        /// The return value of f.
        /// </returns>
        private static T Time<T>(string name, Func<T> f)
        {
            Console.WriteLine("Starting {0}", name);

            var timer = new Stopwatch();
            timer.Start();

            var ret = f();

            timer.Stop();
            Console.WriteLine("Finished {0}, elapsed time {1}", name, arg1: timer.Elapsed);

            return ret;
        }
    }
}