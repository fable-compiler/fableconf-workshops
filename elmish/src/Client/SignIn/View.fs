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

let inputField (inputView: Input.Types.Option list -> Fable.Import.React.ReactElement) label icon (field: StringField) (msg: string -> Msg) dispatch =
    Field.field_div [ ]
      [ yield Label.label [ ]
                [ str label ]
        yield Control.control_div [ Control.hasIconLeft ]
                [ Icon.faIcon [ Icon.isSmall ] icon
                  inputView [ Input.defaultValue field.Value
                              Input.props [ OnChange (fun ev -> msg !!ev.target?value |> dispatch) ] ] ]
        if field.Error.IsSome then
            yield Help.help [ Help.isDanger ]
                    [ str field.Error.Value ] ]

let textField =
    inputField Input.text

let passwordField =
    inputField Input.password

let root model dispatch =
    Columns.columns [ Columns.isCentered ]
        [ Column.column [ Column.Width.isHalf ]
            [ form [ ]
                [ textField "Email" Fa.EnvelopeO model.FormData.Email ChangeEmail dispatch
                  passwordField "Password" Fa.Key model.FormData.Password ChangePassword dispatch

                  Field.field_div [ Field.isGrouped
                                    Field.Types.CustomClass Fulma.BulmaClasses.Bulma.Properties.Float.IsPulledRight ]
                    [ Control.control_div [ ]
                        [ Button.button [ yield Button.isPrimary
                                          yield Button.onClick (fun _ -> dispatch Submit)
                                          if model.IsLoading then
                                            yield Button.isLoading ]
                            [ str "Sign in" ] ] ] ] ] ]
