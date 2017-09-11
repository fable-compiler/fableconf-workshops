[<RequireQualifiedAccess>]
module Elmish.Worker.Program

open System
open Fable.Import.Browser
open Shared

let inline private requestFrame f =
    window.requestAnimationFrame(FrameRequestCallback f) |> ignore


let withPhysicsWorker
    workerUrl (buffer: 'T[])
    (receive: 'T[]->'msg) (send: float->'model->'T[]->bool)
    (program:Elmish.Program<_,'model,'msg,_>) =

    // TODO: Is there a better way to handle this?
    let mutable model = Unchecked.defaultof<'model>

    let mutable animating = true
    let bufferRef = ref (Some buffer)
    let worker = Worker.Create(workerUrl)

    let rec animate dispatch prevts last t =
        program.view model dispatch
        // Don't use timestep if difference is too big,
        // for example when user comes back from another tab
        let timestep = t - last
        match !bufferRef with
        | Some buffer when timestep > 0. && (prevts = 0. || timestep < prevts * 10.) ->
            animating <- send timestep model buffer
            if animating then
                transferArray buffer worker
                bufferRef := None
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
            if send 0. model buffer then
                requestFrame (animate dispatch 0. 0.)

    { program with init = init; setState = setState }