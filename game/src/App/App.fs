module App.Main

open Shared
open State
open Elmish
open Elmish.Worker
open Elmish.Debug
open Fable.Import

let inline receiveBuffer (ar: float[]) =
    // In Debug mode, copy the array to avoid issues with the debugger
    #if DEBUG
    Array.copy ar |> Physics
    #else
    Physics ar
    #endif

let subscribeToAnimationPause (toggle: unit->unit) =
    Browser.window.addEventListener_keyup(fun ev ->
        if ev.keyCode = 80. then // [P]ause
            toggle()
        null)

let init() =
    let info = View.initCanvas()
    let buffer = State.createPhysicsBuffer()

    Program.mkProgram (fun () -> State.initModel 1) State.update (View.render info)
    |> Program.withPhysicsWorker
        Init.workerURL buffer
        State.sendWorkerMessage
        receiveBuffer
        subscribeToAnimationPause
    #if DEBUG
    |> Program.withDebuggerDebounce 200
    #endif
    |> Program.run

init()