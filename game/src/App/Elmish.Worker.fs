[<RequireQualifiedAccess>]
module Elmish.Worker.Program

open System
open Fable.Import
open Shared

let inline private requestFrame f =
    Browser.window.requestAnimationFrame(Browser.FrameRequestCallback f) |> ignore

let withPhysicsWorker
    workerUrl (buffer: float[])
    (receive: float[]->'msg) (send: float->'model->float[]->'workerMsg)
    (program:Elmish.Program<_,'model,'msg,_>) =

    // TODO: Is there a better way to handle this?
    let mutable model = Unchecked.defaultof<'model>

    let mutable animating = true
    let bufferRef = ref (Some buffer)
    let worker = Browser.Worker.Create(workerUrl)

    let rec animate dispatch last t =
        if animating then
            program.view model dispatch
            // Make sure the time delta is not too big (can happen if user switches browser tab)
            let timestep = min 100. (t - last)
            match !bufferRef with
            | Some buffer when timestep > 0. ->
                let msg = send timestep model buffer
                postMessageAndTransferBuffer msg buffer worker
                bufferRef := None
            | _ -> ()
            requestFrame (animate dispatch t)

    let init arg =
        let subscribeWorker dispatch =
            observeWorker worker
            |> Observable.add (fun buffer ->
                bufferRef := Some buffer
                receive buffer |> dispatch)
        let subscribeAnimation dispatch =
            requestFrame (animate dispatch 0.)
        // TODO: Let the user decide the event to toggle animation
        let subscribePauseKey dispatch =
            Browser.window.addEventListener_keyup(fun ev ->
                if ev.keyCode = 80. then // [P]ause
                    animating <- not animating
                    if animating then
                        requestFrame (animate dispatch 0.)
                null)
        let model, cmd = program.init arg
        model, cmd @ [subscribeWorker; subscribeAnimation; subscribePauseKey]

    let setState m dispatch =
        model <- m
        if not animating then
            program.view model dispatch

    { program with init = init; setState = setState }