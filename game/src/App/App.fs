module App.Main

open Shared
open State
open Elmish
open Elmish.Worker
open Elmish.Debug

let mutable private totalTime = 0.

// Stop animation after a few seconds
// so we can use the debugger
let sendBuffer timestep _ (buffer: float[]) =
    if totalTime < 8000. then
        totalTime <- totalTime + timestep
        // Add timesteps (in seconds) to array head
        buffer.[0] <- timestep / 1000.
        true
    else false

let inline receiveBuffer (ar: float[]) =
    // In Debug mode, copy the array to avoid
    // issues with the debugger
    #if DEBUG
    Array.copy ar |> Physics
    #else
    Physics ar
    #endif

let init() =
    // Data array. Contains all our data we need for rendering: a 2D position and an angle per body.
    // It will be sent back and forth from the main thread and the worker thread. When
    // it's sent from the worker, it's filled with position data of all bodies.
    let buffer = Array.zeroCreate (Init.N * 3 + 1)
    let ctx, w, h = View.initCanvas()

    Program.mkProgram State.initModel State.update (View.render (ctx, w, h))
    |> Program.withPhysicsWorker Init.workerURL buffer receiveBuffer sendBuffer
    #if DEBUG
    |> Program.withDebuggerDebounce 200
    #endif
    |> Program.run

init()