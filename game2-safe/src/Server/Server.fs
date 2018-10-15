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

let highScores =
    System.Collections.Concurrent.ConcurrentBag<(string * int)>()

highScores.Add("beat me !", 1)

module List =
    let limit n (xs : list<_>) =
        if xs.Length > n then List.take n xs else xs

let getHighScores() : Task<HighScores> =
    task {
        return
            highScores
            |> Seq.toList
            |> List.sortByDescending snd
            |> List.limit 10
    }

let submitHighScore (newScore : Score) : Task<HighScores> =
    task {
        highScores.Add newScore
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
