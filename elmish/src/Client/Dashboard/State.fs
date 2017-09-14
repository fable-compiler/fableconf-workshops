module Dashboard.State

open Elmish
open Types

let init ()  =
    { Messages = [] }

let update msg model =
    match msg with
    | ChangeStr str ->
      model, Cmd.none
