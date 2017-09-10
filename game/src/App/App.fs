module App

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Browser
open Shared

type IBox =
    abstract X: float
    abstract Y: float
    abstract Angle: float
    abstract Width: float
    abstract Height: float

type Box(x, y, angle) =
    member val X_: float = x with get, set
    member val Y_: float = y with get, set
    member val Angle_: float = angle with get, set
    interface IBox with
        member this.X = this.X_
        member this.Y = this.Y_
        member this.Angle = this.Angle_
        member this.Width = Init.boxWidth
        member this.Height = Init.boxHeight

type IModel =
    abstract Boxes: IBox seq
    abstract Initialized: bool

type Model(boxes) =
    member val Boxes_: Box[] = boxes
    member val Initialized_ = false with get, set
    interface IModel with
        member this.Boxes = this.Boxes_ |> Seq.cast<IBox>
        member this.Initialized = this.Initialized_

type Msg =
    | Physics of float[]

let initModel(): IModel =
    let boxes = Array.init Init.N (fun _ -> Box(0.,0.,0.))
    upcast Model(boxes)

let update (model: IModel) (msg: Msg) =
    let model = model :?> Model
    match msg with
    | Physics data ->
        if not model.Initialized_ then
            model.Initialized_ <- true
        for i=0 to (Init.N - 1) do
            let box = model.Boxes_.[i]
            box.X_     <- data.[i * 3 + 1]
            box.Y_     <- data.[i * 3 + 2]
            box.Angle_ <- data.[i * 3 + 3]
    model //, []

let initCanvas() =
    let canvas = document.getElementsByTagName_canvas().[0]
    let ctx = canvas.getContext_2d()
    ctx.lineWidth <- Init.canvasLineWidth
    ctx, canvas.width, canvas.height

let drawBodies(ctx: CanvasRenderingContext2D, model: IModel) =
    // Draw all bodies. Skip the first one, it's the ground plane
    for box in Seq.skip 1 model.Boxes do
        ctx.beginPath()
        ctx.save()
        // Translate to the center of the box
        ctx.translate(box.X, box.Y)
        // Rotate to the box body frame
        ctx.rotate(box.Angle)
        ctx.rect(-box.Width/2., -box.Height/2., box.Width, box.Height)
        ctx.stroke()
        ctx.restore()

let render(ctx: CanvasRenderingContext2D, canvasWidth: float, canvasHeight: float, model: IModel) =
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
    drawBodies(ctx, model)
    // Restore transform
    ctx.restore()

let init() =
    // Data array. Contains all our data we need for rendering: a 2D position and an angle per body.
    // It will be sent back and forth from the main thread and the worker thread. When
    // it's sent from the worker, it's filled with position data of all bodies.
    let dataRef = Array.zeroCreate (Init.N * 3 + 1) |> Some |> ref

    let mutable model = initModel()
    let worker = Worker.Create(Init.workerURL)
    observeWorker worker
    |> Observable.add (fun ar ->
        model <- update model (Physics ar)
        dataRef := Some ar)

    let ctx, w, h = initCanvas()
    let rec animate prevTimestep last t =
        if model.Initialized then
            render(ctx, w, h, model)
        // Don't use timestep if difference is too big,
        // for example when user comes back from another tab
        let timestep = t - last
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