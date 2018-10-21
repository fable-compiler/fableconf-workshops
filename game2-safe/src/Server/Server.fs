open System
open System.IO
open System.Threading.Tasks
open System.Text

open Microsoft.Extensions.DependencyInjection
open FSharp.Control.Tasks.V2
open Giraffe
open Saturn
open Shared

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Microsoft.WindowsAzure.Storage
open Microsoft.WindowsAzure.Storage.Blob
open Newtonsoft.Json

let tryGetEnv = System.Environment.GetEnvironmentVariable >> function null | "" -> None | x -> Some x
let publicPath = tryGetEnv "public_path" |> Option.defaultValue "../Client/public" |> Path.GetFullPath
let storageAccount = tryGetEnv "STORAGE_CONNECTIONSTRING" |> Option.defaultValue "UseDevelopmentStorage=true" |> CloudStorageAccount.Parse
let port = 8085us

let blobClient = storageAccount.CreateCloudBlobClient()
let containerName = "scores-container"
let blobRef = blobClient.GetContainerReference containerName

module Json =
    open Fable.Remoting

    let jsonConverter = Json.FableJsonConverter () :> JsonConverter
    let serialize value = JsonConvert.SerializeObject(value, [|jsonConverter|])
    let deserialize json = JsonConvert.DeserializeObject<_>(json, [|jsonConverter|])

let blob =
    task {
        let! _ = blobRef.CreateIfNotExistsAsync ()
        let file = blobRef.GetBlockBlobReference "scores.json"
        let! exists = file.ExistsAsync()
        if not exists then do! file.UploadTextAsync "[]"
        return file
    }
    |> Async.AwaitTask
    |> Async.RunSynchronously

let getScores () : Task<Scores> =
    task {
        let! json = blob.DownloadTextAsync()
        return Json.deserialize json
    }

let leaseTime = TimeSpan.FromSeconds 15.

let addScore (score : Score) =
    task {
        let! lease = blob.AcquireLeaseAsync(Nullable.op_Implicit leaseTime, null)
        let! scores = getScores ()
        let newScores = score :: scores
        let json = Json.serialize newScores
        let condition = AccessCondition.GenerateLeaseCondition lease
        do! blob.UploadTextAsync(json, condition, null, null)
        do! blob.ReleaseLeaseAsync condition
        return newScores
    }

let highScores (scores : Scores) : Scores =
    let sorted = List.sortByDescending snd scores
    if sorted.Length > HighScores.limit then
        List.take HighScores.limit sorted
    else
        sorted

let getHighScores() : Task<Scores> =
    task {
        let! scores = getScores()
        return highScores scores
    }

let submitHighScore (name, score) : Task<Scores> =
    task {
        let! scores = addScore (name, score)
        return highScores scores
    }

let counterApi = {
    getHighScores = getHighScores >> Async.AwaitTask
    submitHighScore = submitHighScore >> Async.AwaitTask
}

let errorHandler (ex: Exception) (routeInfo: RouteInfo<_>) =
    printfn "Error at %s on method %s: %s\n%s" routeInfo.path routeInfo.methodName ex.Message ex.StackTrace
    Propagate ex

let webApp =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue counterApi
    |> Remoting.withErrorHandler errorHandler
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
