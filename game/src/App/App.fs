module App.Main

open Shared
open Types
open Elmish
open Elmish.Worker

let init() =
    // Data array. Contains all our data we need for rendering: a 2D position and an angle per body.
    // It will be sent back and forth from the main thread and the worker thread. When
    // it's sent from the worker, it's filled with position data of all bodies.
    let buffer = Array.zeroCreate (Init.N * 3 + 1)
    let ctx, w, h = View.initCanvas()

    Program.mkProgram State.initModel State.update (View.render (ctx, w, h))
    |> Program.withPhysicsWorker Init.workerURL buffer Physics (fun ts buf ->
        buf.[0] <- ts / 1000.; buf)
    // #if DEBUG
    // |> Program.withDebugger
    // #endif
    |> Program.run

init()