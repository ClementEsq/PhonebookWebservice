#r @"packages/FAKE/tools/FakeLib.dll"

open Fake
open Fake.Testing
open System.IO
open System

let buildDir = "./build"
let testDir = "./test"
let reportDir = "./output"
let outputDir = "./output"

let version = if isLocalBuild then "1.0.0" else buildVersion

Target "Clean" (fun _ -> CleanDirs [buildDir; testDir; reportDir; outputDir])

Target "BuildSolution" (fun _ ->
    MSBuildWithDefaults "Build" ["./Phonebook.Webservice.sln"]
    |> Log "AppBuild-Output: "
)

Target "CreateNuGet" (fun _ ->
    Paket.Pack (fun p -> 
        { p with 
            ToolPath = ".paket" @@ "paket.exe" 
            Version = version
            OutputPath = outputDir })
)

Target "Default" DoNothing

"Clean"
    ==> "BuildSolution"
    ==> "Default"
    ==> "CreateNuGet"

RunTargetOrDefault "Default"
