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
      map SignIn (s "sign-in")
      map (ReloadToken >> Session) (s "session" <?> stringParam "nextUrl")
      map (AuthPage Dashboard) (s "dashboard")
      map (AuthPage (Admin Index)) (s "admin")
      map (AuthPage (Admin (User AdminUserPage.Index))) (s "admin" </> s "user")
      map (AuthPage (Admin (User AdminUserPage.Create))) (s "admin" </> s "user" </> s "create")
      map (AdminUserPage.Edit >> AdminPage.User >> Admin >> AuthPage) (s "admin" </> s "user" </> i32 </> s "edit")
      map (AuthPage Dashboard) top ]

let urlUpdate (result: Option<Page>) model =
    match result with
    | None ->
        Browser.console.error("Error parsing url")
        model, Navigation.modifyUrl (toHash model.CurrentPage)

    | Some page ->
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
        | SignIn -> { model with CurrentPage = page }, Cmd.none
        | AuthPage authPage ->
            let model = { model with CurrentPage = page }

            match model.Session with
            | Some session ->
                match authPage with
                | Dashboard ->
                    model, Cmd.none
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
          Dashboard = Dashboard.State.init ()
          SignIn = SignIn.State.init ()
          Session = None }

let update msg (model:Model) =
    match msg with
    | AdminMsg msg ->
        let (admin, adminMsg) = Admin.Dispatcher.State.update msg model.AdminModel
        { model with AdminModel = admin}, Cmd.map AdminMsg adminMsg

    | DashboardMsg msg ->
        let (dashboard, dashboardMsg) = Dashboard.State.update msg model.Dashboard
        { model with Dashboard = dashboard }, Cmd.map DashboardMsg dashboardMsg

    | SignInMsg msg ->
        let (signIn, signInMsg) = SignIn.State.update msg model.SignIn
        { model with SignIn = signIn}, Cmd.map SignInMsg signInMsg
