module App

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import

// Parameters
let dt = 1. / 60.
let N = 100
let zoom = 20.
let boxWidth = 0.5
let boxHeight = 0.5
let workerUrl = "/build/worker.js"

let initCanvas() =
    let canvas = Browser.document.getElementsByTagName_canvas().[0]
    let ctx = canvas.getContext_2d()
    ctx.lineWidth <- 0.05
    ctx, canvas.width, canvas.height

let rec initWorker(url, dataRef: float[] option ref) =
    // Create worker
    let worker = Browser.Worker.Create(url)
    worker.onmessage <- (fun e ->
        printfn "Received: %A" e.data
        dataRef := Some !!e.data
        null)
    createObj["N" ==> N
              "boxWidth" ==> boxWidth
              "boxHeight" ==> boxHeight]
// p2Url : document.location.href.replace(/\/[^/]*$/,"/") + "../../build/p2.js",
    |> worker.postMessage
    sendBuffer(worker, dataRef)
    worker

and sendBuffer(worker: Browser.Worker, dataRef: float[] option ref) =
    match !dataRef with
    | Some ar ->
        worker.postMessage(ar, [|ar?buffer|])
        dataRef := None
    | None -> ()

let drawBodies(ctx: Browser.CanvasRenderingContext2D, data: float[]) =
    // Draw all bodies. Skip the first one, it's the ground plane
    for i=1 to (N-1) do
        ctx.beginPath()
        let x = data.[i * 3 + 0]
        let y = data.[i * 3 + 1]
        let angle = data.[i * 3 + 2]
        ctx.save()
        ctx.translate(x, y) // Translate to the center of the box
        ctx.rotate(angle)   // Rotate to the box body frame
        ctx.rect(-boxWidth/2., -boxHeight/2., boxWidth, boxHeight)
        ctx.stroke()
        ctx.restore()

let render(ctx: Browser.CanvasRenderingContext2D, canvasWidth: float, canvasHeight: float, data: float[]) =
    // Clear the canvas
    ctx.clearRect(0., 0., canvasWidth, canvasHeight)
    // Transform the canvas
    // Note that we need to flip the y axis since Canvas pixel coordinates
    // goes from top to bottom, while physics does the opposite.
    ctx.save()
    ctx.translate(canvasWidth / 2., canvasHeight / 2.)  // Translate to the center
    ctx.scale(zoom, -zoom)       // Zoom in and flip y axis
    // Draw all bodies
    drawBodies(ctx, data)
    // Restore transform
    ctx.restore()

let init() =
    // Data array. Contains all our data we need for rendering: a 2D position and an angle per body.
    // It will be sent back and forth from the main thread and the worker thread. When
    // it's sent from the worker, it's filled with position data of all bodies.
    let data = Array.zeroCreate (N * 3) |> Some |> ref
    let worker = initWorker(workerUrl, data)
    let ctx, w, h = initCanvas()
    let rec animate _ =
        Browser.FrameRequestCallback(animate)
        |> Browser.window.requestAnimationFrame
        |> ignore
        match !data with
        | Some ar ->
            render(ctx, w, h, ar)
            sendBuffer(worker, data)
        | None -> ()
    animate 0.

init()