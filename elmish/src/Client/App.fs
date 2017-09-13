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

importAll "./sass/main.sass"

let genNavbarItem txt refPage currentPage =
    Navbar.item_a [ if refPage = currentPage then
                        yield Navbar.Item.isActive
                    yield Navbar.Item.props [ Href (toHash refPage) ] ]
        [ str txt ]

let navbarAdminLink currentPage =
    let isActive =
        match currentPage with
        | Admin _ -> true
        | _ -> false
    Navbar.link_a [ if isActive then
                        yield Navbar.Link.isActive
                    yield Navbar.Link.props [ Href (toHash (Admin AdminPage.Index)) ] ]
        [ str "Admin" ]

let navbar currentPage =
    Navbar.navbar [ Navbar.props [ Data("test", "maxime") ] ]
        [ Navbar.brand_a [ Fulma.Common.GenericOption.Props [ Href (toHash Home)] ]
            [ img [ Src "/img/logo.svg"
                    Style [ Height "60px" ] ] ]
          Navbar.item_a [
              if currentPage = Home then
                yield Navbar.Item.isActive
              yield Navbar.Item.props [ Href (toHash Home) ]
           ]
            [ str "Home" ]
          Navbar.menu [ ]
            [ Navbar.start_div [ ]
                [ Navbar.item_div [ Navbar.Item.hasDropdown
                                    Navbar.Item.isHoverable ]
                    [ navbarAdminLink currentPage
                      Navbar.dropdown_div [ ]
                        [ genNavbarItem "Users" (Admin (AdminPage.User AdminUserPage.Index)) currentPage ] ] ] ] ]


let root (model: Model) dispatch =

    // let pageHtml =
    //     function
    //     | Page.About -> Info.View.root
    //     | Counter -> Counter.View.root model.Counter (CounterMsg >> dispatch)
    //     | CounterList -> CounterList.View.root model.CounterList (CounterListMsg >> dispatch)
    //     | Home -> Home.View.root model.Home (HomeMsg >> dispatch)

    let pageHtml =
        function
        | Home -> Home.View.root model.Home (HomeMsg >> dispatch)
        | Admin adminPage -> Admin.Dispatcher.View.root model.AdminModel adminPage (AdminMsg >> dispatch)

    div
        [ ClassName "container" ]
        [ navbar model.CurrentPage
          section [ ClassName "section" ]
            [ ]
          pageHtml model.CurrentPage
        ]

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
