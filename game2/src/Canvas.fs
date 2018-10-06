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
type MsgUpdate<'Msg, 'Model> = 'Model -> 'Msg list -> float -> float -> 'Model
type TimeUpdate<'Model> = 'Model -> float -> 'Model
type View<'Model> = 'Model -> Context -> float -> unit

type Canvas =
    static member Path(ctx: Context, [<System.ParamArray>] points: _[]) =
        let mutable init = false
        ctx.beginPath()
        for (x, y) in points do
            if not init then
                init <- true
                ctx.moveTo(x, y)
            else
                ctx.lineTo(x, y)
        ctx.fill()

    static member Circle(this: Context, x, y, radius) =
        this.beginPath()
        this.arc(x, y, radius, 0., 2. * System.Math.PI, false)
        this.fill()

    static member inline WithContext(ctx: Context, render: Context->unit) =
        ctx.save()
        render ctx
        ctx.restore()

    static member Start<'Msg,'Model>
            (canvasId: string, init: 'Model,
             msgUpdate: MsgUpdate<'Msg, 'Model>, timeUpdate: TimeUpdate<'Model>,
             view: View<'Model>, ?subscribe: Browser.HTMLCanvasElement->('Msg->unit)->'Model->unit) =

        let mainLoop: MainLoop = JsInterop.importAll "mainloop.js"
        let canvasEl = Browser.document.getElementById(canvasId) :?> Browser.HTMLCanvasElement
        let ctx = canvasEl.getContext_2d()
        let msgs = ResizeArray()
        let mutable model = init
        let dispatch msg = msgs.Add(msg)
        subscribe |> Option.iter (fun f -> f canvasEl dispatch model)
        mainLoop
            .setBegin(fun timestamp delta ->
                let msgs2 = Seq.toList msgs
                msgs.Clear()
                model <- msgUpdate model msgs2 timestamp delta)
            .setUpdate(fun delta ->
                model <- timeUpdate model delta)
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
