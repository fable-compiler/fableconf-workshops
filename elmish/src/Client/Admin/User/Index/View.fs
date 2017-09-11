module Admin.User.Index.View

open Fable.Core
open Fable.Core.JsInterop
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Types
open Fulma.Elements
open Fulma.Extra.FontAwesome
open Elmish.Browser.Navigation

let userRow dispatch (user : Shared.Types.User) =
    tr [ ]
       [ td [ ] [ str user.Firstname ]
         td [ ] [ str user.Surname ]
         td [ ] [ str user.Email ]
         td [ ] [ a [ OnClick (fun _ -> ShowDetail user.Id |> dispatch)  ]
                    [ Icon.faIcon [ Icon.isSmall ] Fa.Pencil ] ] ]

let root model dispatch =
    div [ ]
        [ Table.table [ Table.isStripped ]
            [ thead [ ]
                [ tr [ ]
                    [ th [ ] [ str "Firsntame" ]
                      th [ ] [ str "Surname" ]
                      th [ ] [ str "Email" ]
                      th [ ] [ ] ] ]
              tbody [ ]
                (model.Users |> List.map (userRow dispatch) ) ] ]
