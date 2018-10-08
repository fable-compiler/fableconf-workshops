module App

open System
open Canvas
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import

module Literals =
    let [<Literal>] BALL_RADIUS = 80.
    let [<Literal>] BALL_X_FORCE = 0.2
    let [<Literal>] BALL_Y_FORCE = -0.2
    let [<Literal>] PLAYER_SIZE = 40.
    let [<Literal>] PLAYER_X_FORCE = 0.001
    let [<Literal>] HARPOON_TIP_SIZE = 16.
    let [<Literal>] HARPOON_STEP = 8.
    let [<Literal>] WORLD_WIDTH = 1000.
    let [<Literal>] WORLD_HEIGHT = 1000.
    let [<Literal>] CANVAS_WIDTH = 600.
    let [<Literal>] CANVAS_HEIGHT = 600.

open Literals

let matter: Matter.IExports = importAll "matter-js"

let inline (~%%) x = jsOptions x

[<RequireQualifiedAccess>]
type Dir =
    | Left
    | Right
    | None

type State =
    | Playing
    | Win
    | Lose

type Model =
    { State: State
      Engine: Matter.Engine
      Balls : Matter.Body[]
      Walls: Matter.Body[]
      Player: Matter.Body
      Harpoon: (float * float) option
      MoveDir: Dir }

type Msg =
    | Collision of Matter.IPair
    | Move of Dir
    | Tick of delta: float
    | Fire

[<RequireQualifiedAccess>]
module Physics =
    let inline vector x y: Matter.Vector = %%(fun o ->
        o.x <- x
        o.y <- y)

    let inline square x y size =
        matter.Bodies.rectangle(x, y, size, size)

    let inline squareWith x y size (opts: Matter.IChamferableBodyDefinition -> unit) =
        matter.Bodies.rectangle(x, y, size, size, %%opts)

    let ball (level: int) dir x y =
        let level = float level
        let radius = BALL_RADIUS / float level
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

    let wall x y width height =
        matter.Bodies.rectangle(x, y, width, height, %%(fun o ->
            o.isStatic <- Some true
        ))

    let castRay bodies (x1, y1) (x2, y2) =
        matter.Query.ray(bodies, vector x1 y1, vector x2 y2)

    let init () =
        let engine = matter.Engine.create()
        let player = square 0. 400. PLAYER_SIZE
        let balls = [|ball 1 Dir.Right 0. -200.|]
        let walls = [|
            wall 0. -(WORLD_HEIGHT / 2.) WORLD_WIDTH 50. // floor
            wall (WORLD_WIDTH / 2.) 0. 50. 1050. // right wall
            wall 0. (WORLD_HEIGHT / 2.) 1000. 50. // ground
            wall -(WORLD_WIDTH / 2.) 0. 50. 1050. // left wall
        |]
        matter.World.add(engine.world, !^[| yield player
                                            yield! balls
                                            yield! walls |]) |> ignore
        { State = Playing
          Engine = engine
          Balls = balls
          Walls = walls
          Player = player
          Harpoon = None
          MoveDir = Dir.None }

    let update (model: Model) (delta: float): unit =
        matter.Engine.update(model.Engine, delta) |> ignore

let init () =
    Physics.init ()

let (|OneIs|_|) (target: Matter.Body) (pair: Matter.IPair) =
    if pair.bodyA = target
    then Some pair.bodyB
    elif pair.bodyB = target
    then Some pair.bodyA
    else None

let (|OneMaybe|_|) (target: Matter.Body option) (pair: Matter.IPair) =
    match target with
    | None -> None
    | Some target ->
        if pair.bodyA = target
        then Some pair.bodyB
        elif pair.bodyB = target
        then Some pair.bodyA
        else None

let (|Ball|_|) (body: Matter.Body) =
    if body.label.StartsWith("BALL")
    then int body.label.[4..] |> Some
    else None

let update (model: Model) = function
    | Collision (OneIs model.Player (Ball _)) ->
        { model with State = Lose }
    | Collision _ -> model
    | Fire ->
        match model.Harpoon with
        | Some _ -> model // Do nothing
        | None ->
            let harpoon = (model.Player.position.x, model.Player.position.y - PLAYER_SIZE)
            { model with Harpoon = Some harpoon }
    | Move dir ->
        { model with MoveDir = dir }
    // TODO: Pass object from Canvas manager to stop/resume animation instead of just ignoring ticks
    | Tick _ when model.State <> Playing ->
        model
    | Tick delta ->
        // Move player
        match model.MoveDir with
        | Dir.None -> ()
        | Dir.Left -> matter.Body.applyForce(model.Player, model.Player.position, Physics.vector -0.005 0.)
        | Dir.Right -> matter.Body.applyForce(model.Player, model.Player.position, Physics.vector 0.005 0.)
        // Update physics: Mutates model
        Physics.update model delta
        // Move or destroy harpoon
        match model.Harpoon with
        | None -> model
        | Some (x, y) ->
            // Check if harpoon string touches any ball
            let collisions = Physics.castRay model.Balls (x, y) (x, WORLD_HEIGHT / 2.)
            let balls, removeHarpoon =
                if collisions.Length = 0
                then model.Balls, false
                else
                    (model.Balls, collisions) ||> Array.fold (fun balls col ->
                        match col.bodyA with
                        | Ball level as ball ->
                            matter.Composite.remove(model.Engine.world, !^ball) |> ignore
                            let balls = balls |> Array.filter (fun b -> b <> ball)
                            if level < 4 then
                                let level = level * 2
                                let balls2 = [|
                                    Physics.ball level Dir.Right ball.position.x ball.position.y
                                    Physics.ball level Dir.Left ball.position.x ball.position.y
                                |]
                                matter.World.add(model.Engine.world, !^balls2) |> ignore
                                Array.append balls balls2
                            else balls
                        | _ -> balls), true
            if balls.Length = 0
            then { model with State = Win }
            elif removeHarpoon || y <= -(WORLD_HEIGHT / 2.)
            then { model with Balls = balls; Harpoon = None }
            else { model with Balls = balls; Harpoon = Some(x, y - HARPOON_STEP) }

let renderCircle (ctx: Context) style (ball: Matter.Body) =
    Canvas.Circle(ctx, style, ball.position.x, ball.position.y, ball.circleRadius)

let renderShape (ctx: Context) style (shape: Matter.Body) =
    let vertices = shape.vertices |> Array.map (fun v -> v.x, v.y)
    Canvas.Shape(ctx, style, vertices)

let renderSquare (ctx: Context) style size (x, y) =
    Canvas.Square(ctx, style, x, y, size)

let renderLine (ctx: Context) style (x1, y1) (x2, y2) =
    Canvas.Line(ctx, style, x1, y1, x2, y2)

let view (model : Model) (ctx: Context) _interpolationPercentage =
    let zoom = min (CANVAS_WIDTH / WORLD_WIDTH) (CANVAS_HEIGHT / WORLD_HEIGHT)
    ctx.clearRect(0., 0., CANVAS_WIDTH, CANVAS_HEIGHT)
    ctx.save()
    // Translate to the center
    ctx.translate(CANVAS_WIDTH / 2., CANVAS_HEIGHT / 2.)
    // Apply zoom
    // ctx.lineWidth <- 2. / zoom
    ctx.scale(zoom, zoom)
    // Draw state
    match model.State with
    | Playing -> ()
    | Win -> Canvas.Text(ctx, !^"white", "YOU WIN", 0., -(WORLD_HEIGHT / 3.))
    | Lose -> Canvas.Text(ctx, !^"white", "GAME OVER", 0., -(WORLD_HEIGHT / 3.))
    // Draw player
    renderShape ctx !^"yellow" model.Player
    // Draw harpoon
    match model.Harpoon with
    | None -> ()
    | Some (x, y) ->
        renderSquare ctx !^"yellow" HARPOON_TIP_SIZE (x, y)
        renderLine ctx !^"white" (x, y) (x, WORLD_HEIGHT / 2.)
    // Draw balls
    for ball in model.Balls do
        renderCircle ctx !^"red" ball
    // Draw walls
    // for wall in model.Walls do
    //     renderShape ctx !^"white" wall
    ctx.restore()

let subscribe (canvas: Browser.HTMLCanvasElement) dispatch (model : Model) =
    canvas.width <- CANVAS_WIDTH
    canvas.height <- CANVAS_HEIGHT
    canvas.style.background <- "black"

    Browser.window.addEventListener_keydown(fun ev ->
        match ev.key.ToLower() with
        | "arrowleft" -> Move Dir.Left |> dispatch
        | "arrowright" -> Move Dir.Right |> dispatch
        | _ -> ())

    Browser.window.addEventListener_keyup(fun ev ->
        match ev.key.ToLower() with
        | "arrowleft" | "arrowright" -> Move Dir.None |> dispatch
        | " " -> dispatch Fire
        | _ -> ())

    matter.Events.on_collisionStart(model.Engine, fun ev ->
        for pair in ev.pairs do
            Collision pair |> dispatch)

// App
Canvas.Start("canvas", init(), Tick, update, view, subscribe)
