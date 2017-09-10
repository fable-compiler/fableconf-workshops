// include Fake libs
#r "./packages/build/FAKE/tools/FakeLib.dll"
#r "System.IO.Compression.FileSystem"
#r "./packages/build/Suave/lib/net40/Suave.dll"

open System
open System.IO
open Fake
open Fake.ReleaseNotesHelper
open Fake.Git
open Fake.Testing.Expecto
open Fake.YarnHelper

let dotnetcliVersion = "2.0.0"

let mutable dotnetExePath = "dotnet"
let runDotnet dir =
    DotNetCli.RunCommand (fun p -> { p with ToolPath = dotnetExePath
                                            WorkingDir = dir
                                            TimeOut =  TimeSpan.FromHours 12. } )
                                            // Extra timeout allow us to run watch mode
                                            // Otherwise, the process is stopped every 30 minutes by default

Target "InstallDotNetCore" (fun _ ->
   dotnetExePath <- DotNetCli.InstallDotNetSDK dotnetcliVersion
)

Target "Clean" (fun _ ->
    !! "/output"
    ++ "/bin"
    ++ "/obj"
    ++ "/src/bin"
    ++ "/src/obj"
  |> CleanDirs
)

// Dependencies

Target "Restore" (fun _ ->
    !! "**/*.fsproj"
    |> Seq.iter (fun s ->
        let dir = IO.Path.GetDirectoryName s
        runDotnet dir "restore")
)

Target "YarnInstall" (fun _ ->
    Yarn (fun p ->
            { p with
                Command = Install Standard
            })
)

Target "Watch" (fun _ ->
    runDotnet "" """fable webpack --port 47283 -- -w"""
)

Target "Build" (fun _ ->
    runDotnet "" """fable webpack --port 47283 -- -p"""
)

// Tests
let testsPath = "./test" |> FullName

// Pattern specifying assemblies to be tested using expecto
let clientTestExecutables = "test/**/bin/**/*Tests*.exe"

Target "InstallTests" (fun _ ->
    !! "Tests/*.fsproj"
    |> Seq.iter (fun s ->
        let dir = IO.Path.GetDirectoryName s
        runDotnet dir "restore")
)

Target "BuildTests" (fun _ ->
    runDotnet testsPath "build"
)

Target "QuickBuildTests" (fun _ ->
    runDotnet testsPath "build"
)

open Suave
open Suave.Filters
open Suave.Operators
open System.Net

let cwd = Directory.GetCurrentDirectory ()

let router basePath =
    choose [
        path "/" >=> Redirection.redirect "/index.html"
        (Files.browse (Path.Combine(basePath, "public")))
    ]

let runClientTest _ =
    startWebServerAsync { defaultConfig with bindings =
                                                [ HttpBinding.create HTTP IPAddress.Loopback 3002us ] } (router cwd)
    |> snd
    |> Async.Start

    !! clientTestExecutables
    |> Expecto (fun p -> { p with Parallel = false } )
    |> ignore

    Async.CancelDefaultToken()

Target "RunClientTests" runClientTest

Target "QuickRunClientTests" runClientTest

// Used to generate zip file to deploy
let release = LoadReleaseNotes "RELEASE_NOTES.md"

Target "All" DoNothing

// Build order
"Clean"
    ==> "InstallDotNetCore"
    ==> "Restore"
    ==> "YarnInstall"
    ==> "Build"
    ==> "InstallTests"
    ==> "BuildTests"
    ==> "RunClientTests"
    ==> "All"

"YarnInstall"
    ==> "Watch"

"QuickBuildTests"
    ==> "QuickRunClientTests"

// start build
RunTargetOrDefault "All"
