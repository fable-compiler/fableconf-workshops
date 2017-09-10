module Admin.User.Index.State

open Elmish
open Types

let init () =
    { Users = [] }, Cmd.ofMsg FetchUsers

let update msg model =
    match msg with
    | ChangeStr str ->
        model, Cmd.none

    | FetchUsers ->
        printfn "Try to fetch user list"
        model, Cmd.none
