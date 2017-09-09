module App

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import

let initCanvas() =
    let canvas = Browser.document.getElementsByTagName_canvas().[0]
    // let w = canvas.width
    // let h = canvas.height
    let ctx = canvas.getContext_2d()
    ctx.lineWidth <- 0.05
    canvas, ctx

// Parameters
let dt = 1. / 60.
let N = 100
let zoom = 20.
let boxWidth = 0.5
let boxHeight = 0.5
let canvas, ctx = initCanvas()
let mutable array: float[] option = None

let rec initWorker() =
    // Data array. Contains all our data we need for rendering: a 2D position and an angle per body.
    // It will be sent back and forth from the main thread and the worker thread. When
    // it's sent from the worker, it's filled with position data of all bodies.
    array <- Some(Array.zeroCreate (N * 3))
    // Create worker
    let worker = Browser.Worker.Create("/build/worker.js")
    worker.onmessage <- (fun e ->
        printfn "Received: %A" e.data
        array <- Some !!e.data
        null)
    createObj["N" ==> N
              "boxWidth" ==> boxWidth
              "boxHeight" ==> boxHeight]
// p2Url : document.location.href.replace(/\/[^/]*$/,"/") + "../../build/p2.js",
    |> worker.postMessage
    sendBuffer(worker)
    worker

and sendBuffer(worker: Browser.Worker) =
    worker.postMessage(array, [|array?buffer|])
    array <- None

let drawBodies() =
    // Draw all bodies. Skip the first one, it's the ground plane
    for i=1 to (N-1) do
        let array = array.Value
        ctx.beginPath()
        let x = array.[i * 3 + 0]
        let y = array.[i * 3 + 1]
        let angle = array.[i * 3 + 2]
        ctx.save()
        ctx.translate(x, y) // Translate to the center of the box
        ctx.rotate(angle)   // Rotate to the box body frame
        ctx.rect(-boxWidth/2., -boxHeight/2., boxWidth, boxHeight)
        ctx.stroke()
        ctx.restore()

let render() =
    // Clear the canvas
    ctx.clearRect(0., 0., canvas.width, canvas.height)
    // Transform the canvas
    // Note that we need to flip the y axis since Canvas pixel coordinates
    // goes from top to bottom, while physics does the opposite.
    ctx.save()
    ctx.translate(canvas.width / 2., canvas.height / 2.)  // Translate to the center
    ctx.scale(zoom, -zoom)       // Zoom in and flip y axis
    // Draw all bodies
    drawBodies()
    // Restore transform
    ctx.restore()

let worker = initWorker()

// Animation loop
let rec animate _ =
    Browser.FrameRequestCallback(animate)
    |> Browser.window.requestAnimationFrame
    |> ignore
    match array with
    | Some ar ->
        render()
        sendBuffer(worker)
    | None -> ()

animate 0.