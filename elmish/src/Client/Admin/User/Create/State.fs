module Admin.User.Create.State

open Elmish
open Elmish.Browser.Navigation
open Types

open Okular
open Okular.Operators
module Validation = Shared.Validation.User

let init () =
    { IsLoading = true
      FormData =
        { Firstname = StringField.Empty
          Surname = StringField.Empty
          Email = StringField.Empty
          Password = StringField.Empty
          PasswordConfirmation = StringField.Empty } }, Cmd.none

let updateErrors formData =
    formData
    |> Lens.set (FormData.EmailLens >-> StringField.ErrorLens) (Validation.verifyEmail formData.Email.Value)
    |> Lens.set (FormData.FirstnameLens >-> StringField.ErrorLens) (Validation.verifyFirstname formData.Firstname.Value)
    |> Lens.set (FormData.SurnameLens >-> StringField.ErrorLens) (Validation.verifySurname formData.Surname.Value)
    |> Lens.set (FormData.PasswordLens >-> StringField.ErrorLens) (Validation.verifyPassword formData.Password.Value)
    |> Lens.set (FormData.PasswordConfirmationLens >-> StringField.ErrorLens)
        (Validation.verifyPasswordConfirmation formData.PasswordConfirmation.Value formData.Password.Value)


let update msg model =
    match msg with
    | NetworkError error ->
        printfn "[Admin.User.Edit][Network error] %s" error.Message
        model, Cmd.none

    | Cancel ->
        let url = AdminUserPage.Index |> AdminPage.User |> Admin |> AuthPage
        model, Navigation.newUrl (toHash url)

    | ChangeFirstname value ->
        model
        |> Lens.set (Model.FormDataLens >-> FormData.FirstnameLens >-> StringField.ValueLens) value
        |> Lens.set (Model.FormDataLens >-> FormData.FirstnameLens >-> StringField.ErrorLens) (Validation.verifyFirstname value) , Cmd.none

    | ChangeSurname value ->
        model
        |> Lens.set (Model.FormDataLens >-> FormData.SurnameLens >-> StringField.ValueLens) value
        |> Lens.set (Model.FormDataLens >-> FormData.SurnameLens >-> StringField.ErrorLens) (Validation.verifySurname value), Cmd.none

    | ChangeEmail value ->
        model
        |> Lens.set (Model.FormDataLens >-> FormData.EmailLens >-> StringField.ValueLens) value
        |> Lens.set (Model.FormDataLens >-> FormData.EmailLens >-> StringField.ErrorLens) (Validation.verifyEmail value), Cmd.none

    | ChangePassword value ->
        model
        |> Lens.set (Model.FormDataLens >-> FormData.PasswordLens >-> StringField.ValueLens) value
        |> Lens.set (Model.FormDataLens >-> FormData.PasswordLens >-> StringField.ErrorLens) (Validation.verifyPassword value), Cmd.none

    | ChangePasswordConfirmation value ->
        let error = Validation.verifyPasswordConfirmation value model.FormData.Password.Value
        model
        |> Lens.set (Model.FormDataLens >-> FormData.PasswordConfirmationLens >-> StringField.ValueLens) value
        |> Lens.set (Model.FormDataLens >-> FormData.PasswordConfirmationLens >-> StringField.ErrorLens) error, Cmd.none

    | Submit ->
        let userData : Shared.Types.UserCreate =
            { Firstname = model.FormData.Firstname.Value
              Surname = model.FormData.Surname.Value
              Email = model.FormData.Email.Value
              Password = model.FormData.Password.Value
              PasswordConfirmation = model.FormData.PasswordConfirmation.Value }

        if Validation.verifyUserCreate userData then
            model, Cmd.ofPromise Rest.createUser userData CreateUserResponse NetworkError
        else
            { model with FormData = updateErrors model.FormData }, Cmd.none

    | CreateUserResponse response ->
        model, Cmd.none
