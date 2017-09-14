module SignIn.State

open Elmish
open Types
open Okular
open Okular.Operators
module Validation = Shared.Validation.SignIn

let init ()=
    { IsLoading = false
      FormData =
        { Email = StringField.Empty
          Password = StringField.Empty } }

let updateErrors formData =
    formData
    |> Lens.set (FormData.EmailLens >-> StringField.ErrorLens) (Validation.verifyEmail formData.Email.Value)
    |> Lens.set (FormData.PasswordLens >-> StringField.ErrorLens) (Validation.verifyPassword formData.Password.Value)

let update msg model =
    match msg with
    | ChangeEmail value ->
        model
        |> Lens.set (Model.FormDataLens >-> FormData.EmailLens >-> StringField.ValueLens) value
        |> Lens.set (Model.FormDataLens >-> FormData.EmailLens >-> StringField.ErrorLens) (Validation.verifyEmail value), Cmd.none

    | ChangePassword value ->
        model
        |> Lens.set (Model.FormDataLens >-> FormData.PasswordLens >-> StringField.ValueLens) value
        |> Lens.set (Model.FormDataLens >-> FormData.PasswordLens >-> StringField.ErrorLens) (Validation.verifyPassword value), Cmd.none

    | Submit ->
        let signInData : Shared.Types.SignInData =
            { Email = model.FormData.Email.Value
              Password = model.FormData.Password.Value }

        if Validation.verifySignInData signInData then
            model, Cmd.ofPromise Rest.signIn signInData SignInSuccess NetworkError
        else
            { model with FormData = updateErrors model.FormData }, Cmd.none

    | NetworkError error -> failwith "Not Implemented"

    | SignInSuccess result ->
        match result.Code with
        | Validation.UserNotFound ->
            Logger.debug "not found"
        | Validation.SignInSuccess ->
            Logger.debug (result.Data :?> Shared.Types.SignInResponse).Token
        | _ -> failwith "Unkown code"

        model, Cmd.none
