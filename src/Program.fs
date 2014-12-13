// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.fs" company="Oswald Maskens">
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
module OCA.Assembler.Program

open OCA.AsmLib
open System

let bytesToUInt32 file bytes = 
    if (bytes |> Array.length) % 4 <> 0 then Fail [ "Binary files must have a multiple of 4 as number of bytes" |> Position.addZero ]
    else 
        let mutable i = 0
        let words = new System.Collections.Generic.List<uint32>()
        while i < bytes.Length do
            words.Add(BitConverter.ToUInt32(bytes, i))
        words
        |> Seq.mapi (fun i word -> 
               word |> Position.add { col = uint32 i + 1u
                                      row = 0u
                                      file = file })
        |> Ok

[<EntryPoint>]
let main argv = 
    if argv.Length <> 4 then printfn "Invalid number of args %i" argv.Length
    else 
        let from = 
            match argv.[0] with
            | "-f" -> 
                System.IO.File.ReadAllText argv.[1]
                |> Source.tokenizeFile argv.[1]
                |> Attempt.bind Assembly.fromTokens
            | "-b" -> 
                System.IO.File.ReadAllBytes argv.[1]
                |> bytesToUInt32 argv.[1]
                |> Attempt.bind Assembly.fromBin
            | _ -> Fail [ sprintf "Invalid args %A" argv |> Position.addZero ]
        match argv.[2] with
        | "-f" -> 
            let res = 
                from
                |> Attempt.bind Assembly.toTokens
                |> Attempt.mapFail Source.formatPositionInError
                |> Attempt.map (List.map Position.remove)
            match res with
            | Ok v -> 
                let asm = v |> List.map (sprintf "%A")
                IO.File.WriteAllLines(argv.[3], asm)
            | Fail msg -> msg |> List.iter (printfn "%s")
        | "-b" -> 
            let res = 
                from
                |> Attempt.bind Assembly.toBin
                |> Attempt.mapFail Source.formatPositionInError
                |> Attempt.map (List.map Position.remove)
            match res with
            | Ok v -> 
                let asm = 
                    v
                    |> Seq.collect BitConverter.GetBytes
                    |> Array.ofSeq
                IO.File.WriteAllBytes(argv.[3], asm)
            | Fail msg -> msg |> List.iter (printfn "%s")
        | "-i" -> 
            let res = 
                from
                |> Attempt.bind Assembly.toIntelHex
                |> Attempt.mapFail Source.formatPositionInError
                |> Attempt.map (List.map Position.remove)
            match res with
            | Ok v -> IO.File.WriteAllLines(argv.[3], v)
            | Fail msg -> msg |> List.iter (printfn "%s")
        | _ -> printfn "Invalid args %A" argv
    0 // return an integer exit code
