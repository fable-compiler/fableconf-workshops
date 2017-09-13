module Admin.User.Edit.State

open Elmish
open Types

open Okular
open Okular.Operators

let init id =
    { IsLoading = true
      FormData =
        { Firstname = StringField.Empty
          Surname = StringField.Empty
          Email = StringField.Empty
          Password = StringField.Empty
          PasswordConfirmation = StringField.Empty }
      UserId = id }, Cmd.ofMsg FetchUser

let update msg model =
    match msg with
    | FetchUser ->
        model, Cmd.ofPromise Rest.getUser model.UserId FetchUserSuccess NetworkError

    | FetchUserSuccess user ->
        { model with FormData = FormData.FromUser user}, Cmd.none

    | NetworkError error ->
        printfn "[Admin.User.Edit][Network error] %s" error.Message
        model, Cmd.none

    | ChangeFirstname value ->
        model
        |> Lens.set (Model.FormDataLens >-> FormData.FirstnameLens >-> StringField.ValueLens) value
        |> Lens.set (Model.FormDataLens >-> FormData.FirstnameLens >-> StringField.ErrorLens) None, Cmd.none

    | ChangeSurname value ->
        model
        |> Lens.set (Model.FormDataLens >-> FormData.SurnameLens >-> StringField.ValueLens) value
        |> Lens.set (Model.FormDataLens >-> FormData.SurnameLens >-> StringField.ErrorLens) None, Cmd.none

    | ChangeEmail value ->
        model
        |> Lens.set (Model.FormDataLens >-> FormData.EmailLens >-> StringField.ValueLens) value
        |> Lens.set (Model.FormDataLens >-> FormData.EmailLens >-> StringField.ErrorLens) None, Cmd.none

    | ChangePassword value ->
        model
        |> Lens.set (Model.FormDataLens >-> FormData.PasswordLens >-> StringField.ValueLens) value
        |> Lens.set (Model.FormDataLens >-> FormData.PasswordLens >-> StringField.ErrorLens) None, Cmd.none

    | ChangePasswordConfirmation value ->
        model
        |> Lens.set (Model.FormDataLens >-> FormData.PasswordConfirmationLens >-> StringField.ValueLens) value
        |> Lens.set (Model.FormDataLens >-> FormData.PasswordConfirmationLens >-> StringField.ErrorLens) None, Cmd.none
