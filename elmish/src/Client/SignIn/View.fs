module SignIn.View

open Fable.Core
open Fable.Core.JsInterop
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Types
open Fulma.Elements
open Fulma.Elements.Form
open Fulma.Extra.FontAwesome
open Fulma.Layouts
open Fulma.BulmaClasses

let emailField field dispatch  =
    Field.field_div [ ]
      [ yield Label.label [ ]
                [ str "Email" ]
        yield Control.control_div [ Control.hasIconLeft ]
                [ Icon.faIcon [ Icon.isSmall ] Fa.EnvelopeO
                  Input.email [ Input.props [ OnChange (fun ev -> ChangeEmail !!ev.target?value |> dispatch) ] ] ]
        if field.Error.IsSome then
            yield Help.help [ Help.isDanger ]
                    [ str field.Error.Value ] ]

let passwordField field dispatch =
    Field.field_div [ ]
      [ yield Label.label [ ]
                [ str "Password" ]
        yield Control.control_div [ Control.hasIconLeft ]
                [ Icon.faIcon [ Icon.isSmall ] Fa.Key
                  Input.password [ Input.props [ OnChange (fun ev -> ChangePassword !!ev.target?value |> dispatch)
                                                 OnKeyDown (fun ev ->
                                                    if ev.key = "Enter" then
                                                        dispatch Submit )
                                               ] ] ]
        if field.Error.IsSome then
            yield Help.help [ Help.isDanger ]
                    [ str field.Error.Value ] ]

let root model dispatch =
    Columns.columns [ Columns.isCentered ]
        [ Column.column [ Column.Width.isHalf ]
            [ form [ ]
                [ emailField model.FormData.Email dispatch
                  passwordField model.FormData.Password dispatch
                  Field.field_div [ Field.isGrouped
                                    Field.Types.CustomClass Fulma.BulmaClasses.Bulma.Properties.Float.IsPulledRight ]
                    [ Control.control_div [ ]
                        [ Button.button [ yield Button.isPrimary
                                          yield Button.onClick (fun _ -> dispatch Submit)
                                          if model.IsLoading then
                                            yield Button.isLoading ]
                            [ str "Sign in" ] ] ] ] ] ]
