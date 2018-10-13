open System.IO
open System.Threading.Tasks

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open FSharp.Control.Tasks.V2
open Giraffe
open Saturn
open Shared

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Microsoft.WindowsAzure.Storage

let tryGetEnv = System.Environment.GetEnvironmentVariable >> function null | "" -> None | x -> Some x
let publicPath = tryGetEnv "public_path" |> Option.defaultValue "../Client/public" |> Path.GetFullPath
let storageAccount = tryGetEnv "STORAGE_CONNECTIONSTRING" |> Option.defaultValue "UseDevelopmentStorage=true" |> CloudStorageAccount.Parse
let port = 8085us

let highScores = System.Collections.Concurrent.ConcurrentBag<_>(
    [
        "alfonsogarciacaro", 5
        "whitetigle", 4
        "MangelMaxime", 3
        "theimowski", 2
        "(anonymous)", 1
    ]
)

let getHighScores() : Task<_> =
    task {
        return
            highScores
            |> Seq.toList
            |> List.sortByDescending snd
            |> List.take 5
    }

let submitHighScore (name, score) : Task <_> =
    task {
        highScores.Add (name, score)
        return! getHighScores ()
    }

let counterApi = {
    getHighScores = getHighScores >> Async.AwaitTask
    submitHighScore = submitHighScore >> Async.AwaitTask
}

let webApp =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue counterApi
    |> Remoting.buildHttpHandler

let configureAzure (services:IServiceCollection) =
    tryGetEnv "APPINSIGHTS_INSTRUMENTATIONKEY"
    |> Option.map services.AddApplicationInsightsTelemetry
    |> Option.defaultValue services

let app = application {
    url ("http://0.0.0.0:" + port.ToString() + "/")
    use_router webApp
    memory_cache
    use_static publicPath
    service_config configureAzure
    use_gzip
}

run app
