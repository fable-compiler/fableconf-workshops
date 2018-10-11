module Client

open System

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import

open Canvas

open Shared

module Literals =
    let [<Literal>] BALL_RADIUS = 120.
    let [<Literal>] BALL_X_FORCE = 1.
    let [<Literal>] BALL_Y_FORCE = -1.
    let [<Literal>] PLAYER_SIZE = 40.
    let [<Literal>] PLAYER_X_FORCE = 0.005
    let [<Literal>] HARPOON_TIP_SIZE = 16.
    let [<Literal>] HARPOON_STEP = 8.
    let [<Literal>] WORLD_WIDTH = 1000.
    let [<Literal>] WORLD_HEIGHT = 1000.
    let [<Literal>] CANVAS_WIDTH = 600.
    let [<Literal>] CANVAS_HEIGHT = 600.

    let WORLD_BOUND_UPPER = - (WORLD_HEIGHT / 2.)
    let WORLD_BOUND_LOWER = WORLD_HEIGHT / 2.
    let WORLD_BOUND_LEFT = - (WORLD_WIDTH / 2.)
    let WORLD_BOUND_RIGHT = WORLD_WIDTH / 2.

    let TEXT_POSITION = -(WORLD_HEIGHT / 3.)

open Literals

let matter: Matter.IExports = importAll "matter-js"

let inline (~%%) x = jsOptions x

[<RequireQualifiedAccess>]
module Physics =
    let wall x y width height =
        matter.Bodies.rectangle(x, y, width, height, %%(fun o ->
            o.isStatic <- Some true
        ))

    let inline square x y size =
        matter.Bodies.rectangle(x, y, size, size)

    let init () =
        let engine = matter.Engine.create()
        let player = square 0. 400. PLAYER_SIZE
        let walls = [|
            wall 0. WORLD_BOUND_UPPER WORLD_WIDTH 50. // ceiling
            wall WORLD_BOUND_RIGHT 0. 50. 1050. // right wall
            wall 0. WORLD_BOUND_LOWER 1000. 50. // ground
            wall WORLD_BOUND_LEFT 0. 50. 1050. // left wall
        |]
        matter.World.add(
            engine.world,
            !^[| yield player; yield! walls |]) |> ignore

        engine, player

    let update (engine: Matter.Engine) (delta: float): unit =
        matter.Engine.update(engine, delta) |> ignore

type Model =
    { Engine : Matter.Engine
      Player : Matter.Body }

type Msg =
    | Tick of delta : float

let init () =
    let engine, player = Physics.init ()
    { Engine = engine
      Player = player }

let update (model: Model) = function
    | Tick delta ->
        Physics.update model.Engine delta
        model

let renderShape (ctx: Context) style (shape: Matter.Body) =
    let vertices = shape.vertices |> Array.map (fun v -> v.x, v.y)
    Canvas.Shape(ctx, style, vertices)

let view (model : Model) (ctx: Context) _ =
    let zoom =
        min
            (CANVAS_WIDTH / WORLD_WIDTH)
            (CANVAS_HEIGHT / WORLD_HEIGHT)
    ctx.clearRect(0., 0., CANVAS_WIDTH, CANVAS_HEIGHT)
    ctx.save()
    // Translate to the center
    ctx.translate(CANVAS_WIDTH / 2., CANVAS_HEIGHT / 2.)
    // Apply zoom
    ctx.scale(zoom, zoom)

    renderShape ctx !^"yellow" model.Player

    ctx.restore()

let subscribe (canvas: Browser.HTMLCanvasElement) dispatch (model : Model) =
    canvas.width <- CANVAS_WIDTH
    canvas.height <- CANVAS_HEIGHT
    canvas.style.background <- "black"

Canvas.Start("canvas", init(), Tick, update, view, subscribe)