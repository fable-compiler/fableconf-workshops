﻿module App.State

open System
open Shared
open Fable.Import.Browser

// Type has mutable fields for performance.
// We could hide this behind an interface,
// but records serialize much better with the debugger.
type Ship =
    { Id: Guid
      mutable X: float
      mutable Y: float
      mutable Angle: float }

type Mace =
    { Id: Guid
      mutable X: float
      mutable Y: float }

type Asteroid =
    { Id: Guid
      mutable X: float
      mutable Y: float
      mutable Angle: float
      Vertices: (float*float)[] }

type ControlKeys =
    { mutable Up: bool
      mutable Left: bool
      mutable Right: bool }

type Model =
    { Ship: Ship
      Mace: Mace
      Asteroids: Asteroid[]
      Level: int
      Keys: ControlKeys
      Initialized: bool }

type Msg =
    | KeyUp of code: float
    | KeyDown of code: float
    | Physics of float[] * Collision[]

/// Data array. Contains all our data we need for rendering.
/// It will be sent back and forth from the main thread and the worker thread.
/// When it's sent from the worker, it's filled with position data of all bodies.
let createPhysicsBuffer(): float[] =
    Array.zeroCreate (3 (* ship *) + 3 (* mace *) + (3 * Init.maxLevel) (* asteroids *))

let sendWorkerMessage timestep (model: Model) (buffer: float[]) =
    let ids =
        Array.map (fun a -> a.Id) model.Asteroids
        |> Array.append [| model.Ship.Id; model.Mace.Id |]
    { Buffer = buffer
      Ids = ids
      Timestep = timestep / 1000. // Convert to ms
      Level = model.Level
      KeyUp = model.Keys.Up
      KeyLeft = model.Keys.Left
      KeyRight = model.Keys.Right }

let receiveWorkerMessage (msg: WorkerMsgBack) =
    let buffer = msg.Buffer
    let msg2 =
        // In Debug mode, copy the array to avoid issues with the debugger
        #if DEBUG
        Physics(Array.copy buffer, msg.Collisions)
        #else
        Physics(buffer, msg.Collisions)
        #endif
    buffer, msg2

let createAsteroid(radius: float): Asteroid =
    { Id = Guid.NewGuid()
      X = 0.
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
        { Ship = { Id = Guid.NewGuid(); X=0.; Y=0.; Angle=0. }
          Mace = { Id = Guid.NewGuid(); X=0.; Y=0. }
          Asteroids = Array.init level (fun _ -> createAsteroid radius)
          Level = level
          Keys = { Up=false; Left=false; Right=false }
          Initialized = false }
    model, [subscribeToKeyEvents]

let printCollisions (collisions: Collision[]) (model: Model) =
    let ids =
        Array.mapi (fun i a -> a.Id, "ast" + (string i)) model.Asteroids
        |> Array.append [| (model.Ship.Id, "ship"); (model.Mace.Id, "mace") |]
        |> Map
    for col in collisions do
        match Map.tryFind col.IdA ids, Map.tryFind col.IdB ids with
        | Some nameA, Some nameB ->
            // Note the formatting capabilities: left padding, decimal digits...AnalyserNodeType
            printfn "Collision %5s <-> %5s: %8.3f" nameA nameB col.Multiplier
        | _ -> ()

let update (msg: Msg) (model: Model) =
    let model =
        match msg with
        | Physics(data, collisions) ->
            printCollisions collisions model
            let model =
                if not model.Initialized
                then { model with Initialized = true }
                else model
            model.Ship.X     <- data.[0]
            model.Ship.Y     <- data.[1]
            model.Ship.Angle <- data.[2]
            model.Mace.X     <- data.[3]
            model.Mace.Y     <- data.[4]
            for i = 0 to model.Asteroids.Length - 1 do
                let asteroid = model.Asteroids.[i]
                asteroid.X     <- data.[5 + (i*3)]
                asteroid.Y     <- data.[6 + (i*3)]
                asteroid.Angle <- data.[7 + (i*3)]
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
