module Admin.User.Create.Types

open Okular.Lens

type FormData =
    { Firstname : StringField
      Surname : StringField
      Email : StringField
      Password : StringField
      PasswordConfirmation : StringField }

    static member FirstnameLens =
        { Get = fun (r : FormData) -> r.Firstname
          Set = fun v (r : FormData) -> { r with Firstname = v } }

    static member SurnameLens =
        { Get = fun (r : FormData) -> r.Surname
          Set = fun v (r : FormData) -> { r with Surname = v } }

    static member EmailLens =
        { Get = fun (r : FormData) -> r.Email
          Set = fun v (r : FormData) -> { r with Email = v } }

    static member PasswordLens =
        { Get = fun (r : FormData) -> r.Password
          Set = fun v (r : FormData) -> { r with Password = v } }

    static member PasswordConfirmationLens =
        { Get = fun (r : FormData) -> r.PasswordConfirmation
          Set = fun v (r : FormData) -> { r with PasswordConfirmation = v } }


type Model =
    { IsLoading : bool
      FormData : FormData }

    static member FormDataLens =
        { Get = fun (r : Model) -> r.FormData
          Set = fun v (r : Model) -> { r with FormData = v } }

type Msg =
    | NetworkError of exn
    | ChangeFirstname of string
    | ChangeSurname of string
    | ChangeEmail of string
    | ChangePassword of string
    | ChangePasswordConfirmation of string
    | Cancel
    | Submit
    | CreateUserResponse of obj
