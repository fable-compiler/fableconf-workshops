module Worker

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import

let [<Global>] self: obj = jsNative
let [<Global>] setInterval(f: unit->unit, ms: float): unit = jsNative

type InitOptions =
    abstract N: int
    abstract dt: int
    abstract boxWidth: float
    abstract boxHeight: float
    abstract p2Url: string

// Syntax helpers
let inline (~%) fields = createObj fields
let makeOpts (f: 'T->unit) =
    let opts = createEmpty<'T> in f opts; opts

let timeStep = 1. / 60.
let mutable N = 20
let mutable array: float[] option = None

let interval (world: P2.World) () =
    world.step(timeStep)
    match array with
    | Some ar ->
        for i = 0 to (world.bodies.Length - 1) do
            let b = world.bodies.[i]
            ar.[3 * i + 0] <- b.position.[0]
            ar.[3 * i + 1] <- b.position.[1]
            ar.[3 * i + 2] <- b.angle
        // Send data back to the main thread
        self?postMessage(ar, [|ar?buffer|]) |> ignore
        array <- None
    | None -> ()

let createWorld(initOpts: InitOptions) =
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

self?onmessage <- fun (ev: Browser.MessageEvent) ->
    match ev.data with
    | :? (float array) as ar -> array <- Some ar
    | _ ->
        let world = createWorld(ev.data :?> InitOptions)
        setInterval(interval world, timeStep * 1000.)
