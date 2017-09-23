module Admin.Dispatcher.State

open Elmish
open Types

let init adminPage =
    match adminPage with
    | AdminPage.Index ->
        let (subModel, subCmd) = Admin.Index.State.init ()
        { Model.Empty with Index = Some subModel }, Cmd.map IndexMsg subCmd

    | AdminPage.User userPage ->
        match userPage with
        | AdminUserPage.Index ->
            let (subModel, subCmd) = Admin.User.Index.State.init ()
            { Model.Empty with UserIndex = Some subModel }, Cmd.map UserIndexMsg subCmd

        | AdminUserPage.Create ->
            let (subModel, subCmd) = Admin.User.Create.State.init ()
            { Model.Empty with UserCreate = Some subModel }, Cmd.map UserCreateMsg subCmd

        | AdminUserPage.Edit id ->
            let (subModel, subCmd) = Admin.User.Edit.State.init id
            { Model.Empty with UserEdit = Some subModel }, Cmd.map UserEditMsg subCmd

let update msg model =
    match msg with
    | IndexMsg msg ->
        let (index, msg) = Admin.Index.State.update msg model.Index
        { model with Index = Some index }, Cmd.map IndexMsg msg

    | UserIndexMsg msg ->
        let (userIndex, userIndexMsg) = secureUpdate Admin.User.Index.State.update msg model.UserIndex
        { model with UserIndex = Some userIndex }, Cmd.map UserIndexMsg userIndexMsg

    | UserEditMsg msg ->
        let (userEdit, userEditMsg) = secureUpdate Admin.User.Edit.State.update msg model.UserEdit
        { model with UserEdit = Some userEdit }, Cmd.map UserEditMsg userEditMsg

    | UserCreateMsg msg ->
        let (userCreate, userCreateMsg) = secureUpdate Admin.User.Create.State.update msg model.UserCreate
        { model with UserCreate = Some userCreate }, Cmd.map UserCreateMsg userCreateMsg
