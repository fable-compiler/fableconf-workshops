module Worker

open System
open System.Collections.Generic
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Shared

let [<Global>] self: Browser.Worker = jsNative

let fixedTimestep = 1. / 60.

// Helpers
let makeOpts (f: 'T->unit) =
    let opts = createEmpty<'T> in f opts; opts

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

let updatePhysics (shipBody: P2.Body) (world: P2.World) (buffer: float[]) =
    let timestep = buffer.[0]
    let keyUp    = buffer.[1]
    let keyLeft  = buffer.[2]
    let keyRight = buffer.[3]
    let _ = step timestep world ["impact"]
    // Thrust: add some force in the ship direction
    shipBody.applyForceLocal((0., keyUp * 2.))
    // Set turn velocity of ship
    shipBody.angularVelocity <- (keyLeft - keyRight) * Init.shipTurnSpeed

let fillAndSendArray (shipBody: P2.Body) (buffer: float[]) =
    buffer.[4] <- fst shipBody.interpolatedPosition
    buffer.[5] <- snd shipBody.interpolatedPosition
    buffer.[6] <- shipBody.interpolatedAngle
    transferArray buffer self

let rand() =
    JS.Math.random() - 0.5

let createAsteroidShape(level): P2.Shape =
    upcast P2.Circle(makeOpts(fun o ->
        o.radius <- Init.asteroidRadius * (Init.numAsteroidLevels - float level) / Init.numAsteroidLevels |> Some
        // Belongs to the ASTEROID group
        o.collisionGroup <- Some Init.ASTEROID
        // Can collide with the BULLET or SHIP group
        o.collisionMask <- Some (Init.BULLET ||| Init.SHIP)
    ))

// Adds random .verts to an asteroid body
let addAsteroidVerts(asteroidBody: P2.Body) =
    let radius = (asteroidBody.shapes.[0] :?> P2.Circle).radius
    asteroidBody?verts <-
        [|0. .. (Init.numAsteroidVerts - 1.)|]
        |> Array.map (fun j ->
            let angle = (float j) * 2. * Math.PI / Init.numAsteroidVerts
            let xv = radius * cos(angle) + rand() * radius * 0.4
            let yv = radius * sin(angle) + rand() * radius * 0.4
            xv, yv)

let addAsteroids() =
    for i = 0 to (Init.level - 1) do
        let x = rand() * Init.spaceWidth
        let y = rand() * Init.spaceHeight
        let vx = rand() * Init.maxAsteroidSpeed
        let vy = rand() * Init.maxAsteroidSpeed
        let va = rand() * Init.maxAsteroidSpeed
        // TODO: Avoid the ship
        // Create asteroid body
        let asteroidBody = P2.Body(makeOpts(fun o ->
            o.mass <- Some 10.
            o.position <- Some (x, y)
            o.velocity <- Some (vx, vy)
            o.angularVelocity <- Some va
            //o.damping <- Some 0.
            //o.angularDamping <- Some 0.
        ))
        asteroidBody.addShape(createAsteroidShape(0.))
        // asteroids.push(asteroidBody)
        // addBodies.push(asteroidBody)
        //asteroidBody?level <- 1
        addAsteroidVerts(asteroidBody)

let createWorld(): P2.Body * P2.World =
    // Init physics world
    let world = P2.World(createObj["gravity" ==> (0.,0.)])
    // Turn off friction, we don't need it.
    world.defaultContactMaterial.friction <- 0.
    // Add ship physics
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
    // Init asteroid shapes
    //addAsteroids()
    // Update the text boxes
    //updateLevel()
    //updateLives()
    shipBody, world

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
    let shipBody, world = createWorld()
    observeWorker self
    |> Observable.add (fun (buffer: float[]) ->
        printfn "%f" buffer.[1]
        updatePhysics shipBody world buffer
        fillAndSendArray shipBody buffer)

init()