module OCA.AsmLib.DummyTest

open NUnit.Framework

[<Test>]
let ``Tests should run``() =
    OCA.Assembler.Program.Version() |> shouldEq 3