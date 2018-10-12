module Client

open System

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import

open Canvas

open Shared

module Server =

    open Shared
    open Fable.Remoting.Client

    /// A proxy you can use to talk to server directly
    let api : IGameApi =
      Remoting.createApi()
      |> Remoting.withRouteBuilder Route.builder
      |> Remoting.buildProxy<IGameApi>

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
    let inline vector x y: Matter.Vector = %%(fun o ->
        o.x <- x
        o.y <- y)

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

[<RequireQualifiedAccess>]
type Dir =
    | Left
    | Right


type Point =
    { X : float
      Y : float }

module Point =
    let fromXY x y =
        { X = x
          Y = y }

    let toTuple (p : Point) = p.X, p.Y

    let move (p : Point) (vector : Point) =
        { X = p.X + vector.X
          Y = p.Y + vector.Y }

    let moveVert (y : float) (p : Point) =
        move p { X = 0.; Y = y }

type Section =
    { Start : Point
      End : Point }

type Model =
    { Engine : Matter.Engine
      Player : Matter.Body
      MoveDir : Dir option
      Harpoon : Section option }

type Msg =
    | Tick of delta : float
    | Move of Dir option
    | Fire

let init () =
    let engine, player = Physics.init ()
    { Engine = engine
      Player = player
      MoveDir = None
      Harpoon = None }

let update (model: Model) = function
    | Tick delta ->
        // Move player
        match model.MoveDir with
        | None -> ()
        | Some Dir.Left ->
            matter.Body.applyForce(
                model.Player,
                model.Player.position,
                Physics.vector -PLAYER_X_FORCE 0.)
        | Some Dir.Right ->
            matter.Body.applyForce(
                model.Player,
                model.Player.position,
                Physics.vector PLAYER_X_FORCE 0.)

        let newHarpoon =
            match model.Harpoon with
            | None -> None
            | Some harpoon ->
                if harpoon.End.Y <= WORLD_BOUND_UPPER then
                    None
                else
                    let harpoonEnd =
                        harpoon.End |> Point.moveVert (- HARPOON_STEP)
                    Some { harpoon with End = harpoonEnd }

        Physics.update model.Engine delta
        { model with
            Harpoon = newHarpoon }
    | Move dir ->
        { model with MoveDir = dir }
    | Fire ->
        match model.Harpoon with
        | Some _ -> model // Do nothing
        | None ->
            let start =
                Point.fromXY
                    model.Player.position.x
                    WORLD_BOUND_LOWER
            let _end =
                Point.fromXY
                    model.Player.position.x
                    (model.Player.position.y - PLAYER_SIZE)
            let harpoon =
                { Start = start
                  End = _end }
            { model with Harpoon = Some harpoon }

let renderShape (ctx: Context) style (shape: Matter.Body) =
    let vertices = shape.vertices |> Array.map (fun v -> v.x, v.y)
    Canvas.Shape(ctx, style, vertices)

let renderSquare (ctx: Context) style size (x, y) =
    Canvas.Square(ctx, style, x, y, size)

let renderLine (ctx: Context) style (x1, y1) (x2, y2) =
    Canvas.Line(ctx, style, x1, y1, x2, y2)

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

    // Draw player
    renderShape ctx !^"yellow" model.Player

    // Draw harpoon
    match model.Harpoon with
    | None -> ()
    | Some harpoon ->
        renderSquare ctx !^"yellow" HARPOON_TIP_SIZE (Point.toTuple harpoon.End)
        renderLine ctx !^"white" (Point.toTuple harpoon.Start) (Point.toTuple harpoon.End)

    ctx.restore()

let subscribe (canvas: Browser.HTMLCanvasElement) dispatch (model : Model) =
    canvas.width <- CANVAS_WIDTH
    canvas.height <- CANVAS_HEIGHT
    canvas.style.background <- "black"

    Browser.window.addEventListener_keydown(fun ev ->
        match ev.key.ToLower() with
        | "arrowleft" -> Move (Some Dir.Left) |> dispatch
        | "arrowright" -> Move (Some Dir.Right) |> dispatch
        | " " -> ev.preventDefault ()
        | _ -> ())

    Browser.window.addEventListener_keyup(fun ev ->
        match ev.key.ToLower() with
        | "arrowleft" | "arrowright" -> Move None |> dispatch
        | " " -> dispatch Fire
        | _ -> ())

Canvas.Start("canvas", init(), Tick, update, view, subscribe)

async {
    let! highScores = Server.api.getHighScores ()
    let scores = Browser.document.getElementById "scores"
    let ol = scores.appendChild (Browser.document.createElement "ol")
    for (score, name) in highScores do
        let li = Browser.document.createElement "li"
        li.innerText <- sprintf "%s: %d" score name
        ol.appendChild li |> ignore
} |> Async.StartImmediate
