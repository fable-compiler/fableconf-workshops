module Admin.User.Edit.View

open Fable.Core
open Fable.Core.JsInterop
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Types
open Fulma.Elements
open Fulma.Extra.FontAwesome

let userRow (user : Shared.Types.User) =
    tr [ ]
       [ td [ ] [ str user.Firstname ]
         td [ ] [ str user.Surname ]
         td [ ] [ str user.Email ]
         td [ ] [ a [ ]
                    [ Icon.faIcon [ Icon.isSmall ] Fa.Eye ] ] ]

let root model dispatch =
    if model.User.IsNone then
        str "loading"
    else
        str model.User.Value.Email
