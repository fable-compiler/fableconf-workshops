module App.State

open System
open Shared
open Fable.Import.Browser

// Type has mutable fields for performance.
// We could hide this behind an interface,
// but records serialize much better with the debugger.
type Ship =
    { mutable X: float
      mutable Y: float
      mutable Angle: float }

type Asteroid =
    { mutable X: float
      mutable Y: float
      mutable Angle: float      
      Vertices: (float*float)[] }
      
type ControlKeys =
    { mutable Up: bool
      mutable Left: bool
      mutable Right: bool }      

type Model =
    { Ship: Ship
      Asteroids: Asteroid[]
      Level: int
      Keys: ControlKeys
      Initialized: bool }

type Msg =
    | KeyUp of code: float
    | KeyDown of code: float
    | Physics of float[]
    
/// Data array. Contains all our data we need for rendering.
/// It will be sent back and forth from the main thread and the worker thread.
/// When it's sent from the worker, it's filled with position data of all bodies.   
let createPhysicsBuffer(): float[] =
    Array.zeroCreate (1 (* level *) + 1 (* timestep *) + 3 (* keys *) + 3 (* ship *) + (3 * Init.maxLevel) (* asteroids *))

let updatePhysicsBuffer timestep (model: Model) (buffer: float[]) =
    // Add level
    buffer.[0] <- float model.Level    
    // Add timesteps (in seconds) to array head
    buffer.[1] <- timestep / 1000.
    // Add keys
    buffer.[2] <- if model.Keys.Up    then 1. else 0.
    buffer.[3] <- if model.Keys.Left  then 1. else 0.
    buffer.[4] <- if model.Keys.Right then 1. else 0.
    true

let createAsteroid(radius: float): Asteroid = 
    { X = 0.
      Y = 0.
      Angle = 0.
      Vertices =
        [|0. .. (Init.numAsteroidVerts - 1.)|]
        |> Array.map (fun j ->
            let angle = (float j) * 2. * Math.PI / Init.numAsteroidVerts
            let xv = radius * cos(angle) + randMinus0_5To0_5() * radius * 0.4
            let yv = radius * sin(angle) + randMinus0_5To0_5() * radius * 0.4
            xv, yv)
    }

let subscribeToKeyEvents dispatch =
    window.addEventListener_keydown(fun ev ->
        KeyDown ev.keyCode |> dispatch; null)
    window.addEventListener_keyup(fun ev ->
        KeyUp ev.keyCode |> dispatch; null)

let initModel(level) =
    let radius = Init.calculateRadius(level)
    let model =
        { Ship = { X=0.; Y=0.; Angle=0. }
          Asteroids = Array.init level (fun _ -> createAsteroid radius)
          Level = level
          Keys = { Up=false; Left=false; Right=false }
          Initialized = false }
    model, [subscribeToKeyEvents]

let update (msg: Msg) (model: Model) =
    let model =
        match msg with
        | Physics data ->
            let model =
                if not model.Initialized
                then { model with Initialized = true }
                else model
            model.Ship.X     <- data.[5]
            model.Ship.Y     <- data.[6]
            model.Ship.Angle <- data.[7]
            for i = 0 to model.Asteroids.Length - 1 do
                let asteroid = model.Asteroids.[i]
                asteroid.X     <- data.[8  + (i*3)]
                asteroid.Y     <- data.[9  + (i*3)]
                asteroid.Angle <- data.[10 + (i*3)]
                //printfn "Asteroid %i: %f-%f-%f" i asteroid.X asteroid.Y asteroid.Angle
            model
        | KeyDown code ->        
            match code with
            | Keys.Up -> model.Keys.Up <- true
            | Keys.Left -> model.Keys.Left <- true
            | Keys.Right -> model.Keys.Right <- true
            | _ -> ()
            model
        | KeyUp code ->
            match code with
            | Keys.Up -> model.Keys.Up <- false
            | Keys.Left -> model.Keys.Left <- false
            | Keys.Right -> model.Keys.Right <- false
            | _ -> ()
            model
    model, []
