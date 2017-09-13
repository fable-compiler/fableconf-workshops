module Worker

open System
open System.Collections.Generic
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Shared

// Helpers
let [<Global>] self: Browser.Worker = jsNative

let makeOpts (f: 'T->unit) =
    let opts = createEmpty<'T> in f opts; opts

// State and fixed values
let fixedTimestep = 1. / 60.

type WorkerModel =
    { World: P2.World
      Ship: P2.Body
      Mace: P2.Body
      Asteroids: P2.Body[] }

let step (timestep: float) (world: P2.World) (events: string list) =
    // TODO: Make sure events elements are distinct?
    let evResults = Dictionary()
    for ev in events do
        evResults.Add(ev, ResizeArray())
    let listeners =
        events |> List.map (fun evName -> evName, (fun e -> evResults.[evName].Add(e)))
    // Subscribe listeners
    for evName, lis in listeners do
        world.on(evName, lis) |> ignore
    // Update physics
    world.step(fixedTimestep, timestep)
    // Unsubscribe listeners
    for evName, lis in listeners do
        world.off(evName, lis) |> ignore
    evResults

let warp (body: P2.Body) =
    let x, y = body.position
    let x =
        if x > Init.spaceWidth / 2.
        then -Init.spaceWidth / 2.
        elif x < -Init.spaceWidth / 2.
        then Init.spaceWidth / 2.
        else x
    let y =
        if y > Init.spaceHeight / 2.
        then -Init.spaceHeight / 2.
        elif y < -Init.spaceHeight / 2.
        then Init.spaceHeight / 2.
        else y
    // Set the previous position too,
    // to not mess up the p2 body interpolation
    body.position <- x, y
    body.previousPosition <- x, y

let updatePhysics (model: WorkerModel) (msg: WorkerMsg) =
    // warp model.Ship
    for asteroid in model.Asteroids do
        warp asteroid

    let timestep = msg.Timestep
    let _ = step timestep model.World ["impact"]

    let keyUp    = if msg.KeyUp    then 1. else 0.
    let keyLeft  = if msg.KeyLeft  then 1. else 0.
    let keyRight = if msg.KeyRight then 1. else 0.
    // Thrust: add some force in the ship direction
    model.Ship.applyForceLocal((0., keyUp * 2.))
    // Set turn velocity of ship
    model.Ship.angularVelocity <- (keyLeft - keyRight) * Init.shipTurnSpeed

let fillAndSendBuffer (model: WorkerModel) (buffer: float[]) =
    buffer.[0] <- fst model.Ship.interpolatedPosition
    buffer.[1] <- snd model.Ship.interpolatedPosition
    buffer.[2] <- model.Ship.interpolatedAngle
    buffer.[3] <- fst model.Mace.interpolatedPosition
    buffer.[4] <- snd model.Mace.interpolatedPosition
    for i = 0 to model.Asteroids.Length - 1 do
        let asteroid = model.Asteroids.[i]
        buffer.[5 + (i*3)] <- fst asteroid.interpolatedPosition
        buffer.[6 + (i*3)] <- snd asteroid.interpolatedPosition
        buffer.[7 + (i*3)] <- asteroid.interpolatedAngle
    postMessageAndTransferBuffer buffer buffer self

let createAsteroidShape(radius: float): P2.Shape =
    upcast P2.Circle(makeOpts(fun o ->
        o.radius <- Some radius
        // Belongs to the ASTEROID group
        o.collisionGroup <- Some Init.ASTEROID
        // Can collide with the MACE or SHIP group
        o.collisionMask <- Some (Init.MACE ||| Init.SHIP)
    ))

let createAsteroids level (world: P2.World) =
    let radius = Init.calculateRadius level
    Array.init level (fun i ->
        let x  = randMinus0_5To0_5() * Init.spaceWidth
        let y  = randMinus0_5To0_5() * Init.spaceHeight
        let vx = randMinus0_5To0_5() * Init.maxAsteroidSpeed
        let vy = randMinus0_5To0_5() * Init.maxAsteroidSpeed
        let va = randMinus0_5To0_5() * Init.maxAsteroidSpeed
        // TODO: Avoid the ship
        // Create asteroid body
        let asteroidBody = P2.Body(makeOpts(fun o ->
            o.mass <- Some 10.
            o.position <- Some (x, y)
            o.velocity <- Some (vx, vy)
            o.angularVelocity <- Some va
            o.damping <- Some 0.
            o.angularDamping <- Some 0.
        ))
        asteroidBody.addShape(createAsteroidShape radius)
        // asteroids.push(asteroidBody)
        // addBodies.push(asteroidBody)
        world.addBody(asteroidBody)
        asteroidBody
    )

let initModel(level: int): WorkerModel =
    let world = P2.World(createObj["gravity" ==> (0.,0.)])
    // Turn off friction, we don't need it.
    world.defaultContactMaterial.friction <- 0.

    // Add ship
    let shipShape = P2.Circle(makeOpts(fun o ->
        o.radius <- Some Init.shipSize
        // Belongs to the SHIP group
        o.collisionGroup <- Some Init.SHIP
        // Only collide with the ASTEROID group
        o.collisionMask <- Some Init.ASTEROID
    ))
    let shipBody = P2.Body(makeOpts(fun o ->
        o.mass <- Some 1.
        o.damping <- Some 0.
        o.angularDamping <- Some 0.
    ))
    shipBody.addShape(shipShape)
    world.addBody(shipBody)

    // Add mace
    let maceShape = P2.Circle(makeOpts(fun o ->
        o.radius <- Some Init.maceSize
        o.collisionGroup <- Some Init.MACE
        o.collisionMask <- Some Init.ASTEROID
    ))
    let maceBody = P2.Body(makeOpts(fun o ->
        o.mass <- Some 5.
        o.position <- Some (0., -Init.maceDistance)
        o.damping <- Some 0.
        o.angularDamping <- Some 0.
    ))
    maceBody.addShape(maceShape)
    world.addBody(maceBody)

    // Spring between ship and mace
    let opts = createObj["restLength" ==> Init.maceDistance
                         "stiffness"  ==> 8.]
    P2.LinearSpring(shipBody, maceBody, opts)
    |> world.addSpring

    { World = world
      Ship = shipBody
      Mace = maceBody
      Asteroids = (createAsteroids level world) }

//let catchImpacts(world: P2.World) =
    //world.on("beginContact", fun evt ->
    //    let bodyA: P2.Body = !!evt?bodyA
    //    let bodyB: P2.Body = !!evt?bodyB

    //    if not hideShip && allowShipCollision && (bodyA = shipBody || bodyB = shipBody) then
    //        // Ship collided with something
    //        allowShipCollision <- false

    //        let otherBody = if bodyA = shipBody then bodyB else bodyA
    //        if asteroids.indexOf(otherBody) <> -1 then
    //            lives <- lives - 1
    //            updateLives()
    //            // Remove the ship body for a while
    //            removeBodies.push(shipBody)
    //            hideShip <- true

    //        if lives > 0 then
    //            failwith "TODO"
    //        else
    //            failwith "TODO: Game over"
    //    // Bullet collision
    //    // elif bulletBodies.indexOf(bodyA) <> -1 || bulletBodies.indexOf(bodyB) <> -1 then
    //) |> ignore

let init() =
    let mutable model = None
    observeWorker self
    |> Observable.add (fun (msg: WorkerMsg) ->
        let model =
            match model with
            | Some m -> m
            | None ->
                let m = initModel msg.Level
                model <- Some m
                m
        updatePhysics model msg
        fillAndSendBuffer model msg.Buffer)

init()