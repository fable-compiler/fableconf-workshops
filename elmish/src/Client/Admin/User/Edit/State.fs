module Admin.User.Edit.State

open Elmish
open Elmish.Browser.Navigation
open Types

open Okular
open Okular.Operators
module Validation = Shared.Validation.User

let init id =
    { IsLoading = true
      FormData =
        { Firstname = StringField.Empty
          Surname = StringField.Empty
          Email = StringField.Empty }
      UserId = id }, Cmd.ofMsg FetchUser

let updateErrors formData =
    formData
    |> Lens.set (FormData.EmailLens >-> StringField.ErrorLens) (Validation.verifyEmail formData.Email.Value)
    |> Lens.set (FormData.FirstnameLens >-> StringField.ErrorLens) (Validation.verifyFirstname formData.Firstname.Value)
    |> Lens.set (FormData.SurnameLens >-> StringField.ErrorLens) (Validation.verifySurname formData.Surname.Value)

let update msg model =
    match msg with
    | FetchUser ->
        model, Cmd.ofPromise Rest.getUser model.UserId FetchUserSuccess NetworkError

    | FetchUserSuccess user ->
        { model with FormData = FormData.FromUser user
                     UserId = user.Id }, Cmd.none

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

    | Submit ->
        let userData : Shared.Types.UserEdit =
            { Firstname = model.FormData.Firstname.Value
              Surname = model.FormData.Surname.Value
              Email = model.FormData.Email.Value }

        if Validation.verifyUserUpdate userData then
            model, Cmd.ofPromise Rest.saveUser (model.UserId, userData) EditUserResponse NetworkError
        else
            { model with FormData = updateErrors model.FormData }, Cmd.none

    | EditUserResponse response ->
        model, Cmd.none
