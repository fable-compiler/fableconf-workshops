[<RequireQualifiedAccess>]
module Physics

open Fable.Core.JsInterop
open Fable.Import

module Consts =
    let [<Literal>] BALL_RADIUS = 120.
    let [<Literal>] BALL_X_FORCE = 1.
    let [<Literal>] BALL_Y_FORCE = -1.5
    let [<Literal>] PLAYER_SIZE = 40.
    let [<Literal>] PLAYER_X_FORCE = 0.005
    let [<Literal>] HARPOON_TIP_SIZE = 16.
    let [<Literal>] HARPOON_STEP = 8.
    let [<Literal>] WORLD_WIDTH = 1000.
    let [<Literal>] WORLD_HEIGHT = 1000.

    let CANVAS_WIDTH, CANVAS_HEIGHT =
        let x = Browser.window.innerWidth - 30.
        x, x

    let WORLD_BOUND_UPPER = - (WORLD_HEIGHT / 2.)
    let WORLD_BOUND_LOWER = WORLD_HEIGHT / 2.
    let WORLD_BOUND_LEFT = - (WORLD_WIDTH / 2.)
    let WORLD_BOUND_RIGHT = WORLD_WIDTH / 2.

    let TEXT_POSITION = -(WORLD_HEIGHT / 3.)

open Consts

let matter: Matter.IExports = importAll "matter-js"

let inline (~%%) x = jsOptions x

let inline vector x y: Matter.Vector = %%(fun o ->
    o.x <- x
    o.y <- y)

let wall x y width height =
    matter.Bodies.rectangle(x, y, width, height, %%(fun o ->
        o.isStatic <- Some true
    ))

let inline square x y size =
    matter.Bodies.rectangle(x, y, size, size)

let ball (level: int) forceX x y =
    let level = float level
    let radius = BALL_RADIUS / (float level |> sqrt)
    let ball = matter.Bodies.circle(x, y, radius, %%(fun o ->
        o.label <- Some (sprintf "BALL%.0f" level)
        o.restitution <- Some 1.
        o.friction <- Some 0.
        o.frictionAir <- Some 0.
    ))
    matter.Body.applyForce(ball, vector x y, vector (forceX / level) (BALL_Y_FORCE / level))
    ball

let (|Ball|_|) (body: Matter.Body) =
    if body.label.StartsWith("BALL")
    then int body.label.[4..] |> Some
    else None

let isBall = function Ball _ -> true | _ -> false


let castRay bodies (x1, y1) (x2, y2) =
    matter.Query.ray(bodies, vector x1 y1, vector x2 y2)

let init () =
    let engine = matter.Engine.create()
    let player = square 0. 400. PLAYER_SIZE
    let balls = [| ball 1 BALL_X_FORCE 0. -200. |]
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

