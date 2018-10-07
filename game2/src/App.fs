module App

open System
open Canvas
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import

module Literals =
    let [<Literal>] BALL_RADIUS = 80.
    let [<Literal>] PLAYER_SIZE = 40.
    let [<Literal>] PLAYER_STEP = 0.001

open Literals

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

[<RequireQualifiedAccess>]
type Dir =
    | Left
    | Right
    | None

type Model =
    { Engine: Matter.Engine
      Balls : Matter.Body[]
      Walls: Matter.Body[]
      Player: Matter.Body
      MoveDir: Dir
      Settings : Settings }

type Msg =
    | PlayerCollision of other: Matter.Body
    | Move of Dir
    | Tick of delta: float

[<RequireQualifiedAccess>]
module Physics =
    let inline vector x y: Matter.Vector = %%(fun o ->
        o.x <- x
        o.y <- y)

    let mkBall (x, y) radius (forceX, forceY) =
        let ball = matter.Bodies.circle(x, y, radius, %%(fun o ->
            o.label <- Some "BALL"
            o.restitution <- Some 1.
            o.friction <- Some 0.
            o.frictionAir <- Some 0.
        ))
        matter.Body.applyForce(ball, vector x y, vector forceX forceY)
        ball

    let mkWall x y width height =
        matter.Bodies.rectangle(x, y, width, height, %%(fun o ->
            o.isStatic <- Some true
        ))

    let init settings =
        let engine = matter.Engine.create()
        let player = matter.Bodies.rectangle(0., 400., PLAYER_SIZE, PLAYER_SIZE)
        let balls = [|mkBall (0., -200.) BALL_RADIUS (0.2, 0.)|]
        let walls = [|
            mkWall 0. 500. 1000. 50. // ground
            mkWall -525. 0. 50. 1050. // left wall
            mkWall 525. 0. 50. 1050. // right wall
            mkWall 0. -500. 1000. 50. // floor
        |]
        matter.World.add(engine.world, !^[| yield player
                                            yield! balls
                                            yield! walls |]) |> ignore
        { Engine = engine
          Balls = balls
          Walls = walls
          Player = player
          MoveDir = Dir.None
          Settings = settings }

    let update (model: Model) (delta: float): unit =
        matter.Engine.update(model.Engine, delta) |> ignore

let init (settings: Settings) =
    Physics.init settings

let update (model: Model) = function
    | PlayerCollision other ->
        if other.label = "BALL" then
            printfn "PLAYER-BALL collision"
        model
    | Move dir ->
        { model with MoveDir = dir }
    | Tick delta ->
        match model.MoveDir with
        | Dir.None -> ()
        | Dir.Left -> matter.Body.applyForce(model.Player, model.Player.position, Physics.vector -0.005 0.)
        | Dir.Right -> matter.Body.applyForce(model.Player, model.Player.position, Physics.vector 0.005 0.)
        Physics.update model delta
        // Just return the same model because Matter mutates the values
        model

let renderCircle (ctx: Context) (ball: Matter.Body) =
    Canvas.Circle(ctx, ball.position.x, ball.position.y, ball.circleRadius)

let renderShape (ctx: Context) (shape: Matter.Body) =
    let vertices = shape.vertices |> Array.map (fun v -> v.x, v.y)
    Canvas.Shape(ctx, vertices)

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
    renderShape ctx model.Player
    for wall in model.Walls do
        renderShape ctx wall
    for ball in model.Balls do
        renderCircle ctx ball
    ctx.restore()

let subscribe (canvas: Browser.HTMLCanvasElement) dispatch (model : Model) =
    canvas.width <- model.Settings.Canvas.Width
    canvas.height <- model.Settings.Canvas.Height

    Browser.window.addEventListener_keydown(fun ev ->
        match ev.key.ToLower() with
        | "arrowleft" -> Move Dir.Left |> dispatch
        | "arrowright" -> Move Dir.Right |> dispatch
        | _ -> ())

    Browser.window.addEventListener_keyup(fun ev ->
        match ev.key.ToLower() with
        | "arrowleft" | "arrowright" -> Move Dir.None |> dispatch
        | _ -> ())

    matter.Events.on_collisionStart(model.Engine, fun ev ->
        for pair in ev.pairs do
            if pair.bodyA = model.Player
            then PlayerCollision pair.bodyB |> dispatch
            elif pair.bodyB = model.Player
            then PlayerCollision pair.bodyA |> dispatch
            else ()
    )

// App
Canvas.Start("canvas", init Settings.Default, Tick, update, view, subscribe)
