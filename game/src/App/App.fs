module App.Main

open Shared
open State
open Elmish
open Elmish.Worker
open Elmish.Debug

let inline receiveBuffer (ar: float[]) =
    // In Debug mode, copy the array to avoid issues with the debugger
    #if DEBUG
    Array.copy ar |> Physics
    #else
    Physics ar
    #endif

let init() =
    let info = View.initCanvas()
    let buffer = State.createPhysicsBuffer()

    Program.mkProgram (fun () -> State.initModel 1) State.update (View.render info)
    |> Program.withPhysicsWorker Init.workerURL buffer receiveBuffer State.updatePhysicsBuffer
    #if DEBUG
    |> Program.withDebuggerDebounce 200
    #endif
    |> Program.run

init()