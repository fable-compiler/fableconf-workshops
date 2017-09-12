module Shared

open System
open Fable.Core
open Fable.Import

module Init =
    let [<Literal>] level = 1
    let [<Literal>] lives = 3
    let [<Literal>] fillStyle = "white"
    let [<Literal>] strokeStyle = "white"    
    let [<Literal>] shipSize = 0.3
    let [<Literal>] shipTurnSpeed = 4.
    let [<Literal>] spaceWidth = 16.
    let [<Literal>] spaceHeight = 9.
    let [<Literal>] maxAsteroidSpeed = 2.
    let [<Literal>] asteroidRadius = 0.9
    let [<Literal>] numAsteroidLevels = 4.
    let [<Literal>] numAsteroidVerts = 10.
    let [<Literal>] workerURL = "/build/worker.js"

    // These will be used in a binary mask so use powers of 2
    let SHIP =     2
    let BULLET =   4
    let ASTEROID = 8

[<RequireQualifiedAccess>]
module Keys =
    let [<Literal>] Space = 32.
    let [<Literal>] Left = 37.
    let [<Literal>] Up = 38.
    let [<Literal>] Right = 39.

// --- HELPERS ---

// Don't make this directly public to prevent
// double evaluation of arguments
[<Emit("$1.postMessage($0, [$0.buffer])")>]
let private transferArrayJs (ar: 'T[]) (worker: Browser.Worker): unit = jsNative

let transferArray (ar: 'T[]) (worker: Browser.Worker): unit =
    transferArrayJs ar worker

/// TODO: This just overwrites the worker.onmessage field, it should
/// check if there's another listener and wrap it if necessary
let observeWorker<'T> (worker: Browser.Worker) =
    { new IObservable<'T> with
        member x.Subscribe w =
            worker.onmessage <- (fun ev ->
                w.OnNext(ev.data :?> 'T)
                null)
            { new IDisposable with
                member x.Dispose() = worker.onmessage <- null } }
