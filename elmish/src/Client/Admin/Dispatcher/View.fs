module Admin.Dispatcher.View

open Fable.Core
open Fable.Core.JsInterop
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Types

let root model (currentPage: AdminPage)  dispatch =
    match currentPage with
    | AdminPage.Index ->
        secureView Admin.Index.View.root model.Index (IndexMsg >> dispatch)

    | AdminPage.User userPage ->
        match userPage with
        | AdminUserPage.Index ->
            secureView Admin.User.Index.View.root model.UserIndex (UserIndexMsg >> dispatch)

        | AdminUserPage.Create ->
            secureView Admin.User.Create.View.root model.UserCreate (UserCreateMsg >> dispatch)

        | AdminUserPage.Edit id ->
            secureView Admin.User.Edit.View.root model.UserEdit (UserEditMsg >> dispatch)
