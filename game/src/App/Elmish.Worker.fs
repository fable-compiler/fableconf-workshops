[<RequireQualifiedAccess>]
module Elmish.Worker.Program

open System
open Fable.Import
open Shared

let inline private requestFrame f =
    Browser.window.requestAnimationFrame(Browser.FrameRequestCallback f) |> ignore


let withPhysicsWorker
    workerUrl (buffer: float[])
    (receive: float[]->'msg) (send: float->'model->float[]->'workerMsg option)
    (program:Elmish.Program<_,'model,'msg,_>) =

    // TODO: Is there a better way to handle this?
    let mutable model = Unchecked.defaultof<'model>

    let mutable animating = true
    let bufferRef = ref (Some buffer)
    let worker = Browser.Worker.Create(workerUrl)

    let rec animate dispatch prevts last t =
        program.view model dispatch
        // Don't use timestep if difference is too big,
        // for example when user comes back from another tab
        let timestep = t - last
        match !bufferRef with
        | Some buffer when timestep > 0. && (prevts = 0. || timestep < prevts * 10.) ->
            match send timestep model buffer with
            | Some msg ->
                postMessageAndTransferBuffer msg buffer worker
                bufferRef := None
                animating <- true
            | None ->
                animating <- false
        | _ -> ()
        if animating then
            requestFrame (animate dispatch timestep t)

    let init arg =
        let subscribeWorker dispatch =
            observeWorker worker
            |> Observable.add (fun buffer ->
                bufferRef := Some buffer
                receive buffer |> dispatch)
        let subscribeAnimation dispatch =
            requestFrame (animate dispatch 0. 0.)
        let model, cmd = program.init arg
        model, cmd @ [subscribeWorker; subscribeAnimation]

    let setState m dispatch =
        model <- m
        if not animating then
            program.view model dispatch
            // Use send function just to check if
            // we should start animating again
            if send 0. model buffer |> Option.isSome then
                requestFrame (animate dispatch 0. 0.)

    { program with init = init; setState = setState }