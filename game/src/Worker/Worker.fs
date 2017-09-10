module Worker

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Shared

let [<Global>] self: Browser.Worker = jsNative
let [<Global>] setInterval(f: unit->unit, ms: float): unit = jsNative

// Syntax helpers
let inline (~%) fields = createObj fields
let makeOpts (f: 'T->unit) =
    let opts = createEmpty<'T> in f opts; opts

let fillAndSendArray (world: P2.World, array: float[]) =
    let timeStep = array.[0]
    world.step(timeStep)
    for i = 0 to (world.bodies.Length - 1) do
        let b = world.bodies.[i]
        array.[3 * i + 1] <- b.position.[0]
        array.[3 * i + 2] <- b.position.[1]
        array.[3 * i + 3] <- b.angle
    // Send data back to the main thread
    Util.transferArray array self

let createWorld(initOpts: Init.Options) =
    let world = P2.World(%["gravity" ==> (0, -5)])
    // Ground plane
    let planeShape = P2.Plane()
    let groundBody = P2.Body(makeOpts(fun o ->
        o.mass <- Some 0.))
    groundBody.addShape(planeShape)
    world.addBody(groundBody)
    // Create N boxes
    for i = 0 to (initOpts.N-1) do
        let boxBody = P2.Body(makeOpts(fun o ->
            o.mass <- Some 1.
            o.position <-
                Some [| JS.Math.random() - 0.5
                        initOpts.boxHeight * (float i) + 0.5
                        JS.Math.random() - 0.5 |]))
        let boxShape = P2.Box(makeOpts(fun o ->
            o.width <- Some initOpts.boxWidth
            o.height <- Some initOpts.boxHeight))
        boxBody.addShape(boxShape)
        world.addBody(boxBody)
    world

let init() =
    let world = createWorld(Init.options)
    self.onmessage <- (fun ev ->
        let ar = ev.data :?> float[]
        fillAndSendArray(world, ar)
        null)

init()