module Shared

open System
open Fable.Core
open Fable.Import

module Init =
    let [<Literal>] N = 100
    let [<Literal>] boxWidth = 0.5
    let [<Literal>] boxHeight = 0.5
    let [<Literal>] canvasZoom = 20.
    let [<Literal>] canvasLineWidth = 0.05
    let [<Literal>] workerURL = "/build/worker.js"

// --- HELPERS ---

// Don't make this directly public to preventç
// double evaluation of arguments
[<Emit("$1.postMessage($0, [$0.buffer])")>]
let private transferArrayJs (ar: 'T[]) (worker: Browser.Worker): unit = jsNative

let transferArray (ar: 'T[]) (worker: Browser.Worker): unit =
    transferArrayJs ar worker

type GenericObservable<'T>() =
    let listeners = ResizeArray<IObserver<'T>>()
    member x.Trigger v =
        for lis in listeners do
            lis.OnNext v
    interface IObservable<'T> with
        member x.Subscribe w =
            listeners.Add(w)
            { new IDisposable with
                member x.Dispose() = listeners.Remove(w) |> ignore }

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
