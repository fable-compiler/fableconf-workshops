open System
open System.IO
open System.Threading.Tasks
open System.Text

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open FSharp.Control.Tasks.V2
open Giraffe
open Saturn
open Shared

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Microsoft.WindowsAzure.Storage
open Microsoft.WindowsAzure.Storage.Table
open Microsoft.WindowsAzure.Storage.Blob
open Newtonsoft.Json
open Microsoft.WindowsAzure.Storage
open Microsoft.WindowsAzure.Storage

let tryGetEnv = System.Environment.GetEnvironmentVariable >> function null | "" -> None | x -> Some x
let publicPath = tryGetEnv "public_path" |> Option.defaultValue "../Client/public" |> Path.GetFullPath
let storageAccount = tryGetEnv "STORAGE_CONNECTIONSTRING" |> Option.defaultValue "UseDevelopmentStorage=true" |> CloudStorageAccount.Parse

let blobClient = storageAccount.CreateCloudBlobClient()
let guid = "151be5b4-3cb7-45b7-8737-65e08e8faf63"
let blobRef = blobClient.GetContainerReference guid

let blob =
    task {
        let! _ = blobRef.CreateIfNotExistsAsync ()
        do! blobRef.SetPermissionsAsync
                (BlobContainerPermissions(
                    PublicAccess = BlobContainerPublicAccessType.Blob
                ))
        let file =
            blobRef.GetBlockBlobReference "scores.json"
        let! exists = file.ExistsAsync()
        if not exists then
            do! file.UploadTextAsync "[]"
        return file
    }
    |> Async.AwaitTask
    |> Async.RunSynchronously

let jsonConverter = Fable.Remoting.Json.FableJsonConverter () :> JsonConverter

let leaseTime = TimeSpan.FromSeconds 15.

let getScores () =
    task {
        let b = Array.zeroCreate 65536
        let! count = blob.DownloadToByteArrayAsync(b, 0)
        let json = Encoding.UTF8.GetString(b, 0, count)
        return JsonConvert.DeserializeObject<HighScores>(json, [|jsonConverter|])
    }

let addScore (score : string * int) =
    task {
        let! lease = blob.AcquireLeaseAsync(Nullable.op_Implicit leaseTime, null)
        let! scores = getScores ()
        let scores = score :: scores
        let json = JsonConvert.SerializeObject(scores, [|jsonConverter|])
        let bytes = Encoding.UTF8.GetBytes json
        let leaseCondition = AccessCondition.GenerateLeaseCondition(lease)
        do! blob.UploadFromByteArrayAsync(
                bytes, 0, bytes.Length,
                leaseCondition, BlobRequestOptions(), OperationContext())
        do! blob.ReleaseLeaseAsync(leaseCondition)
        return scores
    }

let port = 8085us

let highScores =
    System.Collections.Concurrent.ConcurrentBag<(string * int)>()

highScores.Add("beat me !", 1)

module List =
    let limit n (xs : list<_>) =
        if xs.Length > n then List.take n xs else xs

let getHighScores() : Task<HighScores> =
    task {
        let! scores = getScores()
        return
            scores
            |> List.sortByDescending snd
            |> List.limit 10
    }

let submitHighScore (name, score) : Task<HighScores> =
    task {
        return! addScore (name, score)
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
