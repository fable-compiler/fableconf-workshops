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

open Physics.Consts

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

    let moveVert (y : float) (p : Point) =
        { p with Y = p.Y + y }

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
      Harpoon : Section option
      State : GameState }

type Msg =
    | Tick of delta : float
    | Move of Dir option
    | Fire
    | Collision of Matter.IPair

let init () =
    let engine, player, balls = Physics.init ()
    { Engine = engine
      Player = player
      Balls = balls
      State = Playing
      MoveDir = None
      Score = 0
      Harpoon = None }

let handleBallShot (level: int) (ball : Matter.Body) (balls : Matter.Body []) =
    let level = level * 2
    let first =
        Physics.ball level BALL_X_FORCE ball.position.x ball.position.y
    let second =
        Physics.ball level -BALL_X_FORCE ball.position.x ball.position.y
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

let gameOver model reset =
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
            reset ()
        else
            reset ()
    } |> Async.StartImmediate

let update (reset) (model: Model) = function
    | _ when model.State = GameOver ->
        model
    | Collision pair ->
        if (pair.bodyA = model.Player && Physics.isBall pair.bodyB) ||
           (pair.bodyB = model.Player && Physics.isBall pair.bodyA) then
           gameOver model reset
           { model with State = GameOver }
        else
            model
    | Tick delta ->
        // Move player
        match model.MoveDir with
        | None -> ()
        | Some Dir.Left ->
            Physics.matter.Body.applyForce(
                model.Player,
                model.Player.position,
                Physics.vector -PLAYER_X_FORCE 0.)
        | Some Dir.Right ->
            Physics.matter.Body.applyForce(
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
                            | Physics.Ball level as ball ->
                                let splitBalls =
                                    handleBallShot level ball balls

                                let newBalls =
                                    balls
                                    |> Array.filter ((<>) ball)
                                    |> Array.append splitBalls

                                Physics.matter.Composite.remove(
                                    model.Engine.world, !^ball) |> ignore
                                Physics.matter.World.add(
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

    Physics.matter.Events.on_collisionStart(model.Engine, fun ev ->
        for pair in ev.pairs do
            Collision pair |> dispatch)

let rec reset () =
    Canvas.Start("canvas", init (), Tick, update reset, view, subscribe)

[<Emit("$0 in $1")>]
let checkIn (listener: string) (o: obj) : bool = jsNative

if not (checkIn "ontouchstart" Browser.window) then
    Browser.window.alert "Sorry, game is only for mobile!"
else
    async {
        let! highScores = Server.api.getHighScores ()
        reset ()
        renderHighScores highScores
    } |> Async.StartImmediate
