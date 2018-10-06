module App

open System
open Canvas
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import

let matter: Matter.IExports = importAll "matter-js"

let inline (~%) x = createObj x
let inline (~%%) x = jsOptions x

type Dimension =
    { Width: float; Height: float }

type Settings =
    { Canvas: Dimension
      Zoom: float }
    static member Default: Settings =
        let canvas = { Width = 600.; Height = 600. }
        { Canvas = canvas
          Zoom = 0.5 }

type Model =
    { Engine: Matter.Engine
      Balls : Matter.Body[]
      Settings : Settings }

type Msg = unit

[<RequireQualifiedAccess>]
module Physics =
    let inline vector x y: Matter.Vector = %%(fun o ->
        o.x <- x
        o.y <- y)

    let init () =
        let rest = 1.775
        let engine = matter.Engine.create()
        // let boxA = matter.Bodies.rectangle(0., 200., 80., 80., %%(fun o ->
        //     o.restitution <- Some rest))
        let boxB = matter.Bodies.rectangle(50., -200., 80., 80., %%(fun o ->
            o.restitution <- Some rest
            o.friction <- Some 0.
            o.frictionAir <- Some 0.
        ))
        matter.Body.applyForce(boxB, vector 50. -200., vector 0.1 0.)
        let ground = matter.Bodies.rectangle(0., 650., 810., 60., %%(fun o ->
            o.isStatic <- Some true ))
        matter.World.add(engine.world, !^[|boxB; ground|]) |> ignore
        engine, [|boxB|]

    let update (model: Model) (delta: float) =
        let engine = matter.Engine.update(model.Engine, delta)
        // Just return the same model because Matter mutates the values
        { model with Engine = engine }

let init (settings: Settings) =
    let engine, balls = Physics.init()
    { Engine = engine
      Balls = balls
      Settings = settings }

let msgUpdate model _msgs _timestamp _delta =
    model

let timeUpdate model delta =
    Physics.update model delta

let renderBall (ctx: Context) (ball: Matter.Body) =
    let v = ball.vertices.[0]
    Canvas.Circle(ctx, v.x, v.y, 40.)

let view (model : Model) (ctx: Context) _interpolationPercentage =
    let st = model.Settings
    ctx.clearRect(0., 0., st.Canvas.Width, st.Canvas.Height)
    ctx.save()
    ctx.lineWidth <- 2. / st.Zoom
    // Translate to the center
    ctx.translate(st.Canvas.Width / 2., st.Canvas.Height / 2.)
    // Zoom in and flip y axis
    ctx.scale(st.Zoom, st.Zoom)
    ctx.fillStyle <- !^"white"
    for ball in model.Balls do
        renderBall ctx ball
    ctx.restore()

let subscribe (canvas: Browser.HTMLCanvasElement) dispatch (model : Model) =
    canvas.width <- model.Settings.Canvas.Width
    canvas.height <- model.Settings.Canvas.Height

// App
Canvas.Start("canvas", init Settings.Default, msgUpdate, timeUpdate, view, subscribe)
