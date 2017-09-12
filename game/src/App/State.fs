module App.State

open Shared

// Type has mutable fields for performance.
// We could hide this behind an interface,
// but records serialize much better with the debugger.
type Ship =
    { mutable X: float
      mutable Y: float
      mutable Angle: float }

type Model =
    { Ship: Ship
      Initialized: bool }

type Msg =
    | Physics of float[]

let initModel() =
    let model =
        { Ship = { X=0.; Y=0.; Angle=0. }
          Initialized = false }
    model, []

let update (msg: Msg) (model: Model) =
    match msg with
    | Physics data ->
        let model =
            if not model.Initialized
            then { model with Initialized = true }
            else model
        //for i=0 to (Init.N - 1) do
            //let box = model.Boxes.[i]
            //box.X     <- data.[i * 3 + 1]
            //box.Y     <- data.[i * 3 + 2]
            //box.Angle <- data.[i * 3 + 3]
        model, []
