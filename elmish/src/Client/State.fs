module App.State

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Fable.Import
open Types
open Okular
open Okular.Operators

let pageParser: Parser<Page->Page,Page> =
    oneOf [
      map Home (s "home")
      map SignIn (s "sign-in")
      map (ReloadToken >> Session) (s "session" <?> stringParam "nextUrl")
      map (AuthPage (Admin Index)) (s "admin")
      map (AuthPage (Admin (User AdminUserPage.Index))) (s "admin" </> s "user")
      map (AuthPage (Admin (User AdminUserPage.Create))) (s "admin" </> s "user" </> s "create")
      map (AdminUserPage.Edit >> AdminPage.User >> Admin >> AuthPage) (s "admin" </> s "user" </> i32 </> s "edit")
      map (AuthPage (Admin Index)) top ]

let urlUpdate (result: Option<Page>) model =
    match result with
    | None ->
        Browser.console.error("Error parsing url")
        model, Navigation.modifyUrl (toHash model.CurrentPage)

    | Some page ->
        let model = { model with CurrentPage = page }

        match page with
        | Session sessionAction ->
            match sessionAction with
            | ReloadToken nextUrl ->
                let url =
                    match nextUrl with
                    | None | Some "" ->
                        let url = AdminPage.Index |> Admin |> AuthPage
                        toHash url
                    | Some url -> url
                { model with Session =
                                Some { Token = LocalStorage.Token } }, Navigation.newUrl url
        | Home | SignIn -> model, Cmd.none
        | AuthPage authPage ->
            match model.Session with
            | Some session ->
                match authPage with
                | Admin adminPage ->
                    let (subModel, subMsg) = Admin.Dispatcher.State.init adminPage
                    { model with AdminModel = subModel }, Cmd.map AdminMsg subMsg

            | None ->
                match LocalStorage.Token with
                | null ->
                    model, Navigation.newUrl (toHash SignIn)
                | token ->
                    { model with Session =
                                    Some { Token = token } }, Navigation.newUrl (toHash page)


let init result =
    urlUpdate result
        { CurrentPage = AuthPage (Admin AdminPage.Index)
          AdminModel = Admin.Dispatcher.Types.Model.Empty
          Home = Home.State.init ()
          SignIn = SignIn.State.init ()
          Session = None }

let update msg (model:Model) =
    match msg with
    | AdminMsg msg ->
        let (admin, adminMsg) = Admin.Dispatcher.State.update msg model.AdminModel
        { model with AdminModel = admin}, Cmd.map AdminMsg adminMsg

    | HomeMsg msg ->
        let home = Home.State.update msg model.Home
        { model with Home =  home }, Cmd.none

    | SignInMsg msg ->
        let (signIn, signInMsg) = SignIn.State.update msg model.SignIn
        { model with SignIn = signIn}, Cmd.map SignInMsg signInMsg
