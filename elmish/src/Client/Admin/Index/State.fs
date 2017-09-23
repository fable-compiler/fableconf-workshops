module Admin.Index.State

open Elmish
open Types

let init () =
    "", Cmd.none

let update msg model =
    match msg with
    | ChangeStr str ->
        str, Cmd.none
