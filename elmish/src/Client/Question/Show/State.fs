module Question.Show.State

open Elmish
open Types

let init ()  =
    Model.Empty, Cmd.ofMsg FetchQuestions

let update msg model =
    match msg with
    | NetworkError error ->
        printfn "[Dashboard.State][Network error] %s" error.Message
        model, Cmd.none

    | FetchQuestions ->
        model, Cmd.ofPromise Rest.getQuestions () FetchQuestionsSuccess NetworkError

    | FetchQuestionsSuccess result ->
        { model with Questions = result |> Array.toList }, Cmd.none
