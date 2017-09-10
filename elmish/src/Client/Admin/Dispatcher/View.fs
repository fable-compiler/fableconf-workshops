module Admin.Dispatcher.View

open Fable.Core
open Fable.Core.JsInterop
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Types

let root model (currentPage: AdminPage)  dispatch =
    match currentPage with
    | AdminPage.Index -> Admin.Index.View.root model.Index (IndexMsg >> dispatch)
    | AdminPage.User userPage ->
        match userPage with
        | AdminUserPage.Index -> Admin.User.Index.View.root model.UserIndex (UserIndexMsg >> dispatch)
        | AdminUserPage.Create -> str "User create"
        | AdminUserPage.Edit id -> str "User edit"
