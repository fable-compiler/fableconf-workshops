module App.View

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Types
open App.State
open Fulma.Layouts
open Fulma.Components
open Fulma.Elements
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma.BulmaClasses

importAll "./sass/main.sass"

let genNavbarItem txt refPage currentPage =
    Navbar.item_a [ if refPage = currentPage then
                        yield Navbar.Item.isActive
                    yield Navbar.Item.props [ Href (toHash refPage) ] ]
        [ str txt ]

let navbarAdminLink currentPage =
    let isActive =
        match currentPage with
        | AuthPage (Admin _) -> true
        | _ -> false

    let url = AdminPage.Index |> Admin |> AuthPage
    Navbar.link_a [ if isActive then
                        yield Navbar.Link.isActive
                    yield Navbar.Link.props [ Href (toHash url) ] ]
        [ str "Admin" ]

let navbar currentPage =
    let dashboardUrl = Dashboard |> AuthPage

    Navbar.navbar [ Navbar.props [ Data("test", "maxime") ] ]
        [ Navbar.brand_a [ Fulma.Common.GenericOption.Props [ Href (toHash dashboardUrl)] ]
            [ img [ Src "/img/logo.svg"
                    Style [ Height "60px" ] ] ]
          Navbar.item_a [
              if currentPage = dashboardUrl then
                yield Navbar.Item.isActive
              yield Navbar.Item.props [ Href (toHash dashboardUrl) ] ]
            [ str "Questions" ]
          Navbar.menu [ ]
            [ Navbar.start_div [ ]
                [ Navbar.item_div [ Navbar.Item.hasDropdown
                                    Navbar.Item.isHoverable ]
                    [ navbarAdminLink currentPage
                      Navbar.dropdown_div [ ]
                        [ genNavbarItem "Users" (AdminUserPage.Index |> AdminPage.User |> Admin |> AuthPage) currentPage ] ] ] ] ]


let root (model: Model) dispatch =

    match model.CurrentPage with
    | Session _ ->
        div [ ]
            [ str "We should never go in this view because Session should never be stored into the CurrentPage value."
              br [ ]
              str "If needed in the future create an overlay loader" ]
    | SignIn ->
        Hero.hero [ Hero.isFullHeight ]
            [ Hero.head [ ]
                [ Container.container [ ]
                    [ Columns.columns [ Columns.isCentered ]
                        [ Column.column [ Column.Width.isHalf ]
                            [ img [ Src "/img/logo.svg" ] ] ]
                      Columns.columns [ Columns.isCentered ]
                        [ Heading.h3 [ Heading.customClass Bulma.Properties.Alignment.HasTextCentered ]
                            [ str "Sign-in to access the application" ] ] ] ]
              Hero.body [  ]
                [ Container.container [ ]
                    [ SignIn.View.root model.SignIn (SignInMsg >> dispatch ) ] ] ]

    | AuthPage authPage ->
        match authPage with
        | Admin adminPage ->
            Admin.Dispatcher.View.root model.AdminModel adminPage (AdminMsg >> dispatch)
        | Dashboard ->
            Dashboard.View.root model.Dashboard (DashboardMsg >> dispatch)
        | Question _ ->
            Question.Show.View.root model.QuestionModel (QuestionMsg >> dispatch)
        |> (fun pageView ->
            Container.container [ ]
                [ navbar model.CurrentPage
                  pageView ]
        )


open Elmish.React
open Elmish.Debug

// App
Program.mkProgram init update root
|> Program.toNavigable (parseHash pageParser) urlUpdate
|> Program.withReact "elmish-app"
#if DEBUG
|> Program.withDebugger
|> Program.withConsoleTrace
#endif
|> Program.run
