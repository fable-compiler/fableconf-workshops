module App.State

open Shared
open Fable.Import.Browser

// Type has mutable fields for performance.
// We could hide this behind an interface,
// but records serialize much better with the debugger.
type Ship =
    { mutable X: float
      mutable Y: float
      mutable Angle: float }
      
type ControlKeys =
    { mutable Up: bool
      mutable Left: bool
      mutable Right: bool }      

type Model =
    { Ship: Ship
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
    Array.zeroCreate (1 (* timestep *) + 3 (* keys *) + 3 (* ship *))

let updatePhysicsBuffer timestep (model: Model) (buffer: float[]) =
    // Add timesteps (in seconds) to array head
    buffer.[0] <- timestep / 1000.
    // Add keys
    buffer.[1] <- if model.Keys.Up    then 1. else 0.
    buffer.[2] <- if model.Keys.Left  then 1. else 0.
    buffer.[3] <- if model.Keys.Right then 1. else 0.
    true

let subscribeToKeyEvents dispatch =
    window.addEventListener_keydown(fun ev ->
        KeyDown ev.keyCode |> dispatch; null)
    window.addEventListener_keyup(fun ev ->
        KeyUp ev.keyCode |> dispatch; null)

let initModel() =
    let model =
        { Ship = { X=0.; Y=0.; Angle=0. }
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
            model.Ship.X     <- data.[4]
            model.Ship.Y     <- data.[5]
            model.Ship.Angle <- data.[6]
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
