[<RequireQualifiedAccess>]
module Elmish.Worker.Program

open System
open Fable.Import
open Shared

let inline private requestFrame f =
    Browser.window.requestAnimationFrame(Browser.FrameRequestCallback f) |> ignore

/// Run Elmish with a web worker that calculate physics for each animation frame
/// - workerUrl: URL of web worker script
/// - buffer: float array to contain physics data, ownership will be transferred between app and worker
/// - send: Produce the message (which must be JSON serializable) to be sent to worker:
///     + time (s) since last frame
///     + current app model
///     + float array that will be transferred to worker
/// - receive: get the physics data and produce a message to dispatch to Elmish
/// - pauseAnimation: subscribe to an event (e.g. a key press) to toggle the animation
/// - program: Elmish program
let withPhysicsWorker
    (workerUrl: string)
    (buffer: float[])
    (send: float->'model->float[]->'workerMsg)
    (receive: float[]->'msg)
    (pauseAnimation: (unit->unit)->unit)
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
        let subscribeAnimationPause dispatch =
            let toggle() =
                animating <- not animating
                if animating then
                    requestFrame (animate dispatch 0.)
            pauseAnimation toggle
        let model, cmd = program.init arg
        model, cmd @ [subscribeWorker; subscribeAnimation; subscribeAnimationPause]

    let setState m dispatch =
        model <- m
        if not animating then
            program.view model dispatch

    { program with init = init; setState = setState }