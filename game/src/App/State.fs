module App.State

open Shared
open Types

type private Box(x, y, angle) =
    member val X_: float = x with get, set
    member val Y_: float = y with get, set
    member val Angle_: float = angle with get, set
    interface IBox with
        member this.X = this.X_
        member this.Y = this.Y_
        member this.Angle = this.Angle_
        member this.Width = Init.boxWidth
        member this.Height = Init.boxHeight

type private Model(boxes) =
    member val Boxes_: Box[] = boxes
    member val Initialized_ = false with get, set
    interface IModel with
        member this.Boxes = this.Boxes_ |> Seq.cast<IBox>
        member this.Initialized = this.Initialized_

let initModel() =
    let boxes = Array.init Init.N (fun _ -> Box(0.,0.,0.))
    Model(boxes) :> IModel, []

let update (msg: Msg) (model: IModel) =
    let model = model :?> Model
    match msg with
    | Physics data ->
        if not model.Initialized_ then
            model.Initialized_ <- true
        for i=0 to (Init.N - 1) do
            let box = model.Boxes_.[i]
            box.X_     <- data.[i * 3 + 1]
            box.Y_     <- data.[i * 3 + 2]
            box.Angle_ <- data.[i * 3 + 3]
    model :> IModel, []
