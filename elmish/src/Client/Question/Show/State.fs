module Question.Show.State

open Elmish
open Types

let init id  =
    Model.Empty, Cmd.ofMsg (FetchDetail id)

let update msg model =
    match msg with
    | NetworkError error ->
        printfn "[Dashboard.State][Network error] %s" error.Message
        model, Cmd.none

    | FetchDetail id ->
        model, Cmd.ofPromise Rest.getDetails id FetchDetailSuccess NetworkError

    | FetchDetailSuccess result ->
        { model with Question = Some result.Question
                     Answers = result.Answsers }, Cmd.none
