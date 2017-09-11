module Admin.User.Edit.State

open Elmish
open Types

let init id =
    { User = None
      UserId = id }, Cmd.ofMsg FetchUser

let update msg model =
    match msg with
    | FetchUser ->
        model, Cmd.ofPromise Rest.getUser model.UserId FetchUserSuccess Error

    | FetchUserSuccess user ->
        { model with User = Some user }, Cmd.none

    | Error error ->
        printfn "%s" error.Message
        model, Cmd.none
