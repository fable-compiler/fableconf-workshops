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

    let CANVAS_WIDTH, CANVAS_HEIGHT =
        if Browser.window.innerWidth < 600. then
            let w = Browser.window.innerWidth
            w - 30., w - 30.
        else
            600. , 600.

    let WORLD_BOUND_UPPER = - (WORLD_HEIGHT / 2.)
    let WORLD_BOUND_LOWER = WORLD_HEIGHT / 2.
    let WORLD_BOUND_LEFT = - (WORLD_WIDTH / 2.)
    let WORLD_BOUND_RIGHT = WORLD_WIDTH / 2.

    let TEXT_POSITION = -(WORLD_HEIGHT / 3.)

open Literals

let matter: Matter.IExports = importAll "matter-js"

let inline (~%%) x = jsOptions x

[<RequireQualifiedAccess>]
type Dir =
    | Left
    | Right

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

    let ball (level: int) dir x y =
        let level = float level
        let radius = BALL_RADIUS / (float level |> sqrt)
        let forceX =
            (match dir with Dir.Left -> BALL_X_FORCE * -1. | _ -> BALL_X_FORCE) / level
        let ball = matter.Bodies.circle(x, y, radius, %%(fun o ->
            o.label <- Some (sprintf "BALL%.0f" level)
            o.restitution <- Some 1.
            o.friction <- Some 0.
            o.frictionAir <- Some 0.
        ))
        matter.Body.applyForce(ball, vector x y, vector forceX (BALL_Y_FORCE / level))
        ball

    let castRay bodies (x1, y1) (x2, y2) =
        matter.Query.ray(bodies, vector x1 y1, vector x2 y2)

    let init () =
        let engine = matter.Engine.create()
        let player = square 0. 400. PLAYER_SIZE
        let balls = [| ball 1 Dir.Right 0. -200. |]
        let walls = [|
            wall 0. WORLD_BOUND_UPPER WORLD_WIDTH 50. // ceiling
            wall WORLD_BOUND_RIGHT 0. 50. 1050. // right wall
            wall 0. WORLD_BOUND_LOWER 1000. 50. // ground
            wall WORLD_BOUND_LEFT 0. 50. 1050. // left wall
        |]
        matter.World.add(
            engine.world,
            !^[| yield player; yield! balls; yield! walls |]) |> ignore

        engine, player, balls

    let update (engine: Matter.Engine) (delta: float): unit =
        matter.Engine.update(engine, delta) |> ignore


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

type GameState =
    | Playing
    | GameOver

type Model =
    { Engine : Matter.Engine
      Player : Matter.Body
      Balls : Matter.Body[]
      MoveDir : Dir option
      Score : int
      HighScores : Scores
      Harpoon : Section option
      State : GameState }

type Msg =
    | Tick of delta : float
    | Move of Dir option
    | Fire
    | Collision of Matter.IPair

let init (scores) =
    let engine, player, balls = Physics.init ()
    { Engine = engine
      Player = player
      Balls = balls
      State = Playing
      MoveDir = None
      Score = 0
      HighScores = scores
      Harpoon = None }

let (|OneIs|_|) (target: Matter.Body) (pair: Matter.IPair) =
    if pair.bodyA = target
    then Some pair.bodyB
    elif pair.bodyB = target
    then Some pair.bodyA
    else None

let (|Ball|_|) (body: Matter.Body) =
    if body.label.StartsWith("BALL")
    then int body.label.[4..] |> Some
    else None

let handleBallShot (level: int) (ball : Matter.Body) (balls : Matter.Body []) =
    let level = level * 2
    let first =
        Physics.ball level Dir.Right ball.position.x ball.position.y
    let second =
        Physics.ball level Dir.Left ball.position.x ball.position.y
    [| first; second |]

let renderHighScores (highScores : Scores) =
    let scores = Browser.document.getElementById "scores"
    match scores.children.[0] with
    | null -> ()
    | ol -> scores.removeChild ol |> ignore
    let ol = scores.appendChild (Browser.document.createElement "ol")
    for (name, score) in highScores |> Seq.sortByDescending snd do
        let li = Browser.document.createElement "li"
        li.innerText <- sprintf "%s: %d points" name score
        ol.appendChild li |> ignore


let update (reset) (model: Model) = function
    | _ when model.State = GameOver ->
        model
    | Collision (OneIs model.Player (Ball _)) ->
        async {
            let! highScores = Server.api.getHighScores ()
            let isHighScore =
                if highScores.Length < 10 then
                    model.Score > 0
                else
                    let lowest =
                        highScores
                        |> Seq.minBy snd
                        |> snd
                    model.Score > lowest
            if isHighScore then
                let name =
                    Browser.window.prompt
                        ((sprintf "High score: %d! Type in your name:" model.Score))
                let name = if String.IsNullOrEmpty name then "(anonymous)" else name
                let! highScores = Server.api.submitHighScore (name, model.Score)
                renderHighScores highScores
                reset highScores
            else
                reset highScores
        } |> Async.StartImmediate

        { model with State = GameOver }
    | Collision _ ->
        model
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

        let balls, score, newHarpoon =
            match model.Harpoon with
            | None -> model.Balls, model.Score, None
            | Some harpoon ->
                // Check if harpoon string touches any ball
                let collisions =
                    Physics.castRay
                        model.Balls
                        (Point.toTuple harpoon.Start)
                        (Point.toTuple harpoon.End)
                let (balls, score), removeHarpoon =
                    if collisions.Length = 0
                    then (model.Balls, model.Score), false
                    else
                        ((model.Balls, model.Score), collisions) ||> Array.fold (fun (balls, score) col ->
                            match col.bodyA with
                            | Ball level as ball ->
                                let splitBalls =
                                    handleBallShot level ball balls

                                let newBalls =
                                    balls
                                    |> Array.filter ((<>) ball)
                                    |> Array.append splitBalls

                                matter.Composite.remove(
                                    model.Engine.world, !^ball) |> ignore
                                matter.World.add(
                                    model.Engine.world, !^splitBalls) |> ignore

                                (newBalls, score + splitBalls.Length / 2)
                            | _ -> (balls, score)), true

                if harpoon.End.Y <= WORLD_BOUND_UPPER then
                    balls, score, None
                elif removeHarpoon then
                    balls, score, None
                else
                    let harpoonEnd =
                        harpoon.End |> Point.moveVert (- HARPOON_STEP)
                    balls, score, Some { harpoon with End = harpoonEnd }

        Physics.update model.Engine delta
        { model with
            Harpoon = newHarpoon
            Balls = balls
            Score = score }
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

let renderCircle (ctx: Context) style (ball: Matter.Body) =
    Canvas.Circle(ctx, style, ball.position.x, ball.position.y, ball.circleRadius)

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

    // Draw balls
    for ball in model.Balls do
        renderCircle ctx !^"red" ball

    // Draw state
    match model.State with
    | Playing ->
        Canvas.Text(ctx, !^"white", sprintf "SCORE: %d" model.Score, 0., TEXT_POSITION)
    | GameOver ->
        Canvas.Text(ctx, !^"white", sprintf "GAME OVER, SCORE: %d" model.Score, 0., TEXT_POSITION)

    ctx.restore()


// Touch / Mouse listeners
[<Emit("$0 in $1")>]
let checkIn (listener: string) (o: obj) : bool = jsNative


let subscribe (canvas: Browser.HTMLCanvasElement) dispatch (model : Model) =
    canvas.width <- CANVAS_WIDTH
    canvas.height <- CANVAS_HEIGHT
    canvas.style.background <- "black"

    let left = Browser.document.getElementById "left"
    let right = Browser.document.getElementById "right"
    let fire = Browser.document.getElementById "fire"

    let buttonWidth = sprintf "%fpx" (CANVAS_WIDTH / 3.)

    left.style.width <- buttonWidth
    right.style.width <- buttonWidth
    fire.style.width <- buttonWidth
    fire.parentElement.style.width <- buttonWidth

    if (checkIn "ontouchstart" left) then
        left.addEventListener_touchstart (fun _ ->
            dispatch (Move (Some Dir.Left)))
        right.addEventListener_touchstart (fun _ ->
            dispatch (Move (Some Dir.Right)))
        left.addEventListener_touchend (fun _ ->
            dispatch (Move None))
        right.addEventListener_touchend (fun _ ->
            dispatch (Move None))
        fire.addEventListener_click (fun _ ->
            dispatch Fire)
    else
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

    matter.Events.on_collisionStart(model.Engine, fun ev ->
        for pair in ev.pairs do
            Collision pair |> dispatch)

let rec reset (highScores) =
    Canvas.Start("canvas", init highScores, Tick, update reset, view, subscribe)

async {
    let! highScores = Server.api.getHighScores ()
    reset (highScores)
    renderHighScores highScores
} |> Async.StartImmediate
