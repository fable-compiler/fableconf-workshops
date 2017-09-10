module Admin.Dispatcher.State

open Elmish
open Types

let init () : Model =
    { Index = Admin.Index.State.init ()
      UserIndex = Admin.User.Index.State.init () }

let update msg model =
    match msg with
    | IndexMsg msg ->
        let (index, msg) = Admin.Index.State.update msg model.Index
        { model with Index = index }, Cmd.map IndexMsg msg

    | UserIndexMsg msg ->
        let (userIndex, userIndexMsg) = Admin.User.Index.State.update msg model.UserIndex
        { model with UserIndex = userIndex }, Cmd.map UserIndexMsg userIndexMsg
