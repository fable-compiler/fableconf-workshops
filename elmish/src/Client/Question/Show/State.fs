module Question.Show.State

open Elmish
open Types
open Okular
open Okular.Operators
module Validation = Shared.Validation.Question

let init id  =
    Model.Empty, Cmd.none

let update msg model =
    match msg with
    | None ->
        model, Cmd.none
