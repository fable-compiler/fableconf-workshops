module Canvas

open Fable.Core
open Fable.Import

type private MainLoop =
    abstract setBegin: (float -> float -> unit) -> MainLoop
    abstract setUpdate: (float -> unit) -> MainLoop
    abstract setDraw: (float -> unit) -> MainLoop
    abstract setEnd: (float -> bool -> unit) -> MainLoop
    abstract start: unit -> unit
    abstract getFPS: unit -> float
    abstract getMaxAllowedFPS: unit -> float
    abstract setMaxAllowedFPS: float -> unit
    abstract getSimulationTimestep: unit -> float
    abstract setSimulationTimestep: float -> unit
    abstract resetFrameDelta: unit -> float

type Context = Browser.CanvasRenderingContext2D
type Update<'Msg, 'Model> = 'Model -> 'Msg -> 'Model
type View<'Model> = 'Model -> Context -> float -> unit

type Canvas =
    static member Text(ctx: Context, style, text, x, y, ?font) =
        ctx.save()
        ctx.textAlign <- "center"
        ctx.font <- defaultArg font "48px Arial"
        ctx.fillStyle <- style
        ctx.fillText(text, x, y)
        ctx.restore()

    static member Shape(ctx: Context, style, [<System.ParamArray>] points: _[]) =
        ctx.save()
        ctx.beginPath()
        let x, y = points.[0]
        ctx.moveTo(x, y)
        for i = 1 to points.Length - 1 do
            let x, y = points.[i]
            ctx.lineTo(x, y)
        ctx.fillStyle <- style
        ctx.fill()
        ctx.restore()

    static member Square(ctx: Context, style, x, y, size) =
        let half = size / 2.
        ctx.save()
        ctx.beginPath()
        ctx.moveTo(x - half, y - half)
        ctx.lineTo(x + half, y - half)
        ctx.lineTo(x + half, y + half)
        ctx.lineTo(x - half, y + half)
        ctx.fillStyle <- style
        ctx.fill()
        ctx.restore()

    static member Line(ctx: Context, style, x1, y1, x2, y2) =
        ctx.save()
        ctx.beginPath()
        ctx.moveTo(x1, y1)
        ctx.lineTo(x2, y2)
        ctx.strokeStyle <- style
        ctx.stroke()
        ctx.restore()

    static member Circle(ctx: Context, style, x, y, radius) =
        ctx.save()
        ctx.beginPath()
        ctx.arc(x, y, radius, 0., 2. * System.Math.PI, false)
        ctx.fillStyle <- style
        ctx.fill()
        ctx.restore()

    static member inline WithContext(ctx: Context, render: Context->unit) =
        ctx.save()
        render ctx
        ctx.restore()

    static member Start<'Msg,'Model>
            (canvasId: string, init: 'Model, tick: float->'Msg, update: Update<'Msg, 'Model>,
             view: View<'Model>, ?subscribe: Browser.HTMLCanvasElement->('Msg->unit)->'Model->unit) =

        let mainLoop: MainLoop = JsInterop.importAll "mainloop.js"
        let canvasEl = Browser.document.getElementById(canvasId) :?> Browser.HTMLCanvasElement
        let ctx = canvasEl.getContext_2d()
        let msgs = ResizeArray()
        let mutable model = init
        let dispatch msg = msgs.Add(msg)
        subscribe |> Option.iter (fun f -> f canvasEl dispatch model)
        mainLoop
            .setBegin(fun _timestamp _delta ->
                let msgs2 = Array.ofSeq msgs
                msgs.Clear()
                model <- (model, msgs2) ||> Array.fold update
            )
            .setUpdate(fun delta ->
                model <- tick delta |> update model)
            .setDraw(fun interpolationPercentage ->
                view model ctx interpolationPercentage)
            // TODO: Make the end function customizable
            // For default behaviour see https://github.com/IceCreamYou/MainLoop.js/blob/a499baae76d4197dbf8178877cbdd2b903a00dc8/demo/index.html#L210
            .setEnd(fun _fps panic ->
                if panic then
                    let discardedTime = mainLoop.resetFrameDelta() |> System.Math.Round
                    Browser.console.warn("Main loop panicked, probably because the browser tab was put in the background. Discarding (ms)", discardedTime))
            .start()
        // mainLoop.setMaxAllowedFPS 20.
