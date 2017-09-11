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
        model, Cmd.ofPromise Rest.getUserList () FetchUsersSuccess Error

    | FetchUsersSuccess users ->
        { model with Users = users |> Array.toList }, Cmd.none

    | Error error ->
        printfn "%s" error.Message
        model, Cmd.none
