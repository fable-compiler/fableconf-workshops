module App

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Browser
open Shared

let initCanvas() =
    let canvas = document.getElementsByTagName_canvas().[0]
    let ctx = canvas.getContext_2d()
    ctx.lineWidth <- Init.canvasLineWidth
    ctx, canvas.width, canvas.height

let drawBodies(ctx: CanvasRenderingContext2D, data: float[]) =
    // Draw all bodies. Skip the first one, it's the ground plane
    for i=1 to (Init.N - 1) do
        ctx.beginPath()
        let x =     data.[i * 3 + 1]
        let y =     data.[i * 3 + 2]
        let angle = data.[i * 3 + 3]
        ctx.save()
        ctx.translate(x, y) // Translate to the center of the box
        ctx.rotate(angle)   // Rotate to the box body frame
        ctx.rect(-Init.boxWidth/2., -Init.boxHeight/2., Init.boxWidth, Init.boxHeight)
        ctx.stroke()
        ctx.restore()

let render(ctx: CanvasRenderingContext2D, canvasWidth: float, canvasHeight: float, data: float[]) =
    // Clear the canvas
    ctx.clearRect(0., 0., canvasWidth, canvasHeight)
    // Transform the canvas
    // Note that we need to flip the y axis since Canvas pixel coordinates
    // goes from top to bottom, while physics does the opposite.
    ctx.save()
    // Translate to the center
    ctx.translate(canvasWidth / 2., canvasHeight / 2.)
    // Zoom in and flip y axis
    ctx.scale(Init.canvasZoom, -Init.canvasZoom)
    // Draw all bodies
    drawBodies(ctx, data)
    // Restore transform
    ctx.restore()

let init() =
    // Data array. Contains all our data we need for rendering: a 2D position and an angle per body.
    // It will be sent back and forth from the main thread and the worker thread. When
    // it's sent from the worker, it's filled with position data of all bodies.
    let dataRef = Array.zeroCreate (Init.N * 3 + 1) |> Some |> ref
    let ctx, w, h = initCanvas()
    let worker = Worker.Create(Init.workerURL)
    observeWorker worker
    |> Observable.add (fun ar -> dataRef := Some ar)

    let rec animate prevTimestep last t =
        match !dataRef with
        | Some ar when ar.[0] > 0. -> render(ctx, w, h, ar)
        | _ -> ()
        let timestep = t - last
        // Don't use timestep if difference is too big,
        // for example when user comes back from another tab
        if timestep < prevTimestep * 10. then
            match !dataRef with
            | Some ar when timestep > 0. ->
                ar.[0] <- timestep / 1000.
                transferArray ar worker
                dataRef := None
            | _ -> ()
        window.requestAnimationFrame(FrameRequestCallback(animate timestep t)) |> ignore

    // Start animation loop
    animate 0. 0. 0.

init()