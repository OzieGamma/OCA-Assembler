// include Fake libs
#r "packages/FAKE/tools/FakeLib.dll"

open Fake
open Fake.AssemblyInfoFile
open Fake.Git

// Directories
let buildDir  = "./build/"
let testDir   = "./testBuild/"
let outputDir = "./output/"


let appProjects  = 
    !! "src/**/*.csproj" 
      ++ "src/**/*.fsproj"

let testProjects = !! "test/**/*.csproj"
                        ++ "test/**/*.fsproj"

// version info
let version = "0.0.1"
let stringVersion = Information.getCurrentSHA1 "."

let asmLibDir = "..\\OCA-AsmLib"

(* ------------------- Download AsmLib ----------------*)

Target "EnsureAsmLib" (fun _ ->
    if not (directoryExists asmLibDir) then
        failwith "AsmLib missing"
)

(* ------------------- Normal targets ----------------*)

Target "Clean" (fun _ -> 
    CleanDirs [buildDir; testDir; outputDir]
)

Target "AssemblyInfo" (fun _ ->
    CreateCSharpAssemblyInfo "./src/Properties/AssemblyInfo.cs"
        [Attribute.Title "OCA-Assembler"
         Attribute.Description ("Assembler for OCA - " + stringVersion)
         Attribute.Product "OCA-Assembler"
         Attribute.Version version
         Attribute.FileVersion version
         Attribute.InternalsVisibleTo "OCA.Assembler.Test"]
)

Target "BuildApp" (fun _ -> 
    MSBuildRelease buildDir "Build" appProjects
        |> Log "AppBuild-Output: "
)

Target "BuildTest" (fun _ ->
    MSBuildDebug testDir "Build" testProjects
        |> Log "TestBuild-Output: "
)

Target "Test" (fun _ ->  
    !! (testDir + "/*.dll")
        |> NUnit (fun p -> 
            {p with 
                DisableShadowCopy = true
                TimeOut = System.TimeSpan.FromMinutes 5.0
                Framework = "4.5"
                Domain = NUnitDomainModel.DefaultDomainModel
                OutputFile = "TestResults.xml"})
)

Target "Deploy" (fun _ ->
    !! (buildDir + "/**/*.*") 
        -- "*.zip" 
        |> Zip buildDir (outputDir + "Assembler." + stringVersion + ".zip")
)

Target "MainBuild" DoNothing

// Build order
"Clean"
  ==> "EnsureAsmLib"
  ==> "AssemblyInfo"
  ==> "BuildApp"
  ==> "BuildTest"
  ==> "Test"
  ==> "Deploy"
  ==> "MainBuild"

// start build
RunTargetOrDefault "MainBuild"