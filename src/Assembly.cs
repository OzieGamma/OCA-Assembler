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
            From(options.InType, options.InputFile)
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
        private static void TryContinue<T>(this Attempt<T> attempt, string phase, Action<T> f)
        {
            if (attempt.IsOk)
            {
                f(((Attempt<T>.Ok)attempt).Item);
            }
            else
            {
                List<string> errors = ((Attempt<T>.Fail)attempt).Item.ToList();

                Console.WriteLine("{0} Errors during {1} phase !!! {2}{2}", errors.Count, phase, Environment.NewLine);

                foreach (string error in errors)
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
            FSharpList<AsmInstr> instructions, 
            AssemblerOptions.OutputType outputType, 
            string outputFile)
        {
            switch (outputType)
            {
                case AssemblerOptions.OutputType.Friendly:
                    SourceModule.ToFriendly(instructions)
                        .TryContinue("to", output => File.WriteAllLines(outputFile, output));
                    break;
                case AssemblerOptions.OutputType.Bin:
                    SourceModule.ToBin(instructions).TryContinue(
                        "to", 
                        output =>
                            {
                                IEnumerable<byte> bytes = output.SelectMany(BitConverter.GetBytes);
                                File.WriteAllBytes(outputFile, bytes.ToArray());
                            });
                    break;
                case AssemblerOptions.OutputType.Intel:
                    SourceModule.ToIntelHex(instructions)
                        .TryContinue("to", output => File.WriteAllLines(outputFile, output));
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
        private static Attempt<FSharpList<AsmInstr>> From(AssemblerOptions.InputType inputType, string inputFile)
        {
            switch (inputType)
            {
                case AssemblerOptions.InputType.Friendly:
                    return SourceModule.FromFriendly(RemoveCommentsAndTrim(File.ReadAllLines(inputFile)).ToFSharpList());

                case AssemblerOptions.InputType.Bin:
                    byte[] bytes = File.ReadAllBytes(inputFile);
                    if (bytes.Length % 4 != 0)
                    {
                        return
                            Attempt<FSharpList<AsmInstr>>.NewFail(
                                new FSharpList<string>("Invalid binary file: " + inputType, FSharpList<string>.Empty));
                    }

                    var words = new List<uint>();
                    for (int i = 0; i < bytes.Length; i += 4)
                    {
                        words.Add(BitConverter.ToUInt32(bytes, i));
                    }

                    return SourceModule.FromBin(words.ToFSharpList());

                default:
                    throw new ArgumentException("Unknown input type" + inputType);
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
            return
                input.Select(_ => new string(_.Trim().TakeWhile(c => c != '#').ToArray()))
                    .Where(_ => _ != string.Empty)
                    .ToList();
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