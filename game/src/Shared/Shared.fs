module Shared

open System
open Fable.Core
open Fable.Import

// Use a Pojo recrod so it serializes well in JS
type [<Pojo>] WorkerMsg =
    { Buffer: float[]
      Timestep: float
      Level: int
      KeyUp: bool
      KeyLeft: bool
      KeyRight: bool }

module Init =
    let [<Literal>] lives = 3
    let [<Literal>] maxLevel = 10
    let [<Literal>] fillStyle = "white"
    let [<Literal>] strokeStyle = "white"
    let [<Literal>] shipSize = 0.3
    let [<Literal>] shipMass = 6.
    let [<Literal>] shipTurnSpeed = 4.
    let [<Literal>] maceSize = 0.5
    let [<Literal>] maceMass = 3.
    let [<Literal>] maceDistance = 2.5
    let [<Literal>] spaceWidth = 16.
    let [<Literal>] spaceHeight = 9.
    let [<Literal>] maxAsteroidSpeed = 2.
    let [<Literal>] asteroidMass = 10.
    let [<Literal>] asteroidRadius = 0.9
    let [<Literal>] numAsteroidVerts = 10.
    let [<Literal>] workerURL = "/build/worker.js"

    // These will be used in a binary mask so use powers of 2
    let SHIP =     2
    let MACE =     4
    let ASTEROID = 8
    let BOUND =    16

    let calculateRadius(level: int) =
        asteroidRadius * (float (maxLevel - level)) / float maxLevel

[<RequireQualifiedAccess>]
module Keys =
    let [<Literal>] Space = 32.
    let [<Literal>] Left = 37.
    let [<Literal>] Up = 38.
    let [<Literal>] Right = 39.

// --- HELPERS ---

// Don't make this directly public to prevent
// double evaluation of arguments
[<Emit("$2.postMessage($0, [$1.buffer])")>]
let private postMessageAndTransferBufferJs (msg: 'Msg) (ar: float[]) (worker: Browser.Worker): unit = jsNative

let postMessageAndTransferBuffer<'Msg> (msg: 'Msg) (ar: float[]) (worker: Browser.Worker): unit =
    postMessageAndTransferBufferJs msg ar worker

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


let private r = Random()

/// Create a random number between -0.5 to 0.5
let randMinus0_5To0_5() =
    r.NextDouble() - 0.5