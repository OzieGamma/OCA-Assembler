// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblerOptions.cs" company="Oswald Maskens">
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
    using System.IO;

    /// <summary>
    ///     The assembler options.
    /// </summary>
    internal struct AssemblerOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblerOptions"/> struct.
        /// </summary>
        /// <param name="inType">
        /// The in Type.
        /// </param>
        /// <param name="outType">
        /// The out Type.
        /// </param>
        /// <param name="inputFile">
        /// The input File.
        /// </param>
        /// <param name="outputFile">
        /// The output file.
        /// </param>
        public AssemblerOptions(InputType inType, OutputType outType, string inputFile, string outputFile)
            : this()
        {
            this.InType = inType;
            this.OutType = outType;
            this.InputFile = inputFile;
            this.OutputFile = outputFile;

            if (inputFile == null || outputFile == null || this.InType == InputType.Invalid
                || this.OutType == OutputType.Invalid || !File.Exists(this.InputFile))
            {
                this.IsValid = false;
            }
            else
            {
                string dirName = Path.GetDirectoryName(outputFile);

                if (dirName == null)
                {
                    this.IsValid = false;
                }
                else
                {
                    if (dirName != string.Empty)
                    {
                        Directory.CreateDirectory(dirName);
                    }

                    this.IsValid = true;
                }
            }
        }

        /// <summary>
        ///     The input type.
        /// </summary>
        internal enum InputType
        {
            /// <summary>
            ///     The invalid.
            /// </summary>
            Invalid, 

            /// <summary>
            ///     The friendly.
            /// </summary>
            Friendly, 

            /// <summary>
            ///     The bin.
            /// </summary>
            Bin
        }

        /// <summary>
        ///     The output type.
        /// </summary>
        internal enum OutputType
        {
            /// <summary>
            ///     The invalid.
            /// </summary>
            Invalid, 

            /// <summary>
            ///     The friendly.
            /// </summary>
            Friendly, 

            /// <summary>
            ///     The bin.
            /// </summary>
            Bin, 

            /// <summary>
            ///     The intel.
            /// </summary>
            Intel
        }

        /// <summary>
        ///     Gets the input type.
        /// </summary>
        public InputType InType { get; private set; }

        /// <summary>
        ///     Gets the output type.
        /// </summary>
        public OutputType OutType { get; private set; }

        /// <summary>
        ///     Gets the input files.
        /// </summary>
        public string InputFile { get; private set; }

        /// <summary>
        ///     Gets the output file.
        /// </summary>
        public string OutputFile { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether is valid.
        /// </summary>
        public bool IsValid { get; private set; }
    }
}