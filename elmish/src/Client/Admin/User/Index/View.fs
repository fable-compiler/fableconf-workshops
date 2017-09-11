module Admin.User.Index.View

open Fable.Core
open Fable.Core.JsInterop
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Types

let root model dispatch =
    div []
        [ for user in model.Users do
            yield str user.Firstname
            yield br []
        ]
