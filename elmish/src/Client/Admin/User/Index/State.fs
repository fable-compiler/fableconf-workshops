module Admin.User.Index.State

open Elmish
open Types

let init () : Model =
    ""

let update msg model =
    match msg with
    | ChangeStr str ->
        str, Cmd.none
