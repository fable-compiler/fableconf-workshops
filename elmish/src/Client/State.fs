module App.State

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Fable.Import.Browser
open Types
open Okular
open Okular.Operators

let pageParser: Parser<Page->Page,Page> =
    oneOf [
      map Home (s "home")
      map (Admin Index) (s "admin")
      map (Admin (User AdminUserPage.Index)) (s "admin" </> s "user")
      map (Admin (User AdminUserPage.Create)) (s "admin" </> s "user" </> s "create")
      map (AdminUserPage.Edit >> AdminPage.User >> Admin) (s "admin" </> s "user" </> i32 </> s "edit")
    ]

let urlUpdate (result: Option<Page>) model =
    match result with
    | None ->
        console.error("Error parsing url")
        model, Navigation.modifyUrl (toHash model.CurrentPage)

    | Some page ->
        let model = { model with CurrentPage = page }

        match page with
        | Home -> model, Cmd.none
        | Admin adminPage ->
            let (subModel, subMsg) = Admin.Dispatcher.State.init adminPage
            { model with AdminModel = subModel }, Cmd.map AdminMsg subMsg

let init result =
    urlUpdate result
        { CurrentPage = Home
          AdminModel = Admin.Dispatcher.Types.Model.Empty// Admin.Dispatcher.State.init()
          Home = Home.State.init() }

let update msg (model:Model) =
    match msg with
    | AdminMsg msg ->
        let (admin, adminMsg) = Admin.Dispatcher.State.update msg model.AdminModel
        { model with AdminModel = admin}, Cmd.map AdminMsg adminMsg

    | HomeMsg msg ->
        let home = Home.State.update msg model.Home
        { model with Home =  home }, Cmd.none