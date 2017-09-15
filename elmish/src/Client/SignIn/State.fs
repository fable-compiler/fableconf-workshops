module SignIn.State

open Elmish
open Elmish.Browser.Navigation
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
            { model with IsLoading = true }, Cmd.ofPromise Rest.signIn signInData SignInSuccess NetworkError
        else
            { model with FormData = updateErrors model.FormData }, Cmd.none

    | NetworkError error ->
        Logger.errorFn "[Sign.State][Network error] %s" error.Message
        { model with IsLoading = false }, Cmd.none

    | SignInSuccess result ->
        match result.Code with
        | Validation.UserNotFound ->
            { model with IsLoading = false }, Cmd.none

        | Validation.SignInSuccess ->
            LocalStorage.Session <- (result.Data :?> Shared.Types.SignInResponse)
            let url = SessionAction.ReloadToken >> Session
            { model with IsLoading = false }, Navigation.newUrl (toHash (url None))

        | code ->
            Logger.errorFn "Unkown code: %O" code
            { model with IsLoading = false }, Cmd.none
