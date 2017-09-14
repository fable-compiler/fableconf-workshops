module Admin.User.Edit.Types

open Okular.Lens

type FormData =
    { Firstname : StringField
      Surname : StringField
      Email : StringField }

    static member FromUser (user : Shared.Types.User) =
        { Firstname = StringField.Initial user.Firstname
          Surname = StringField.Initial user.Surname
          Email = StringField.Initial user.Email }

    static member FirstnameLens =
        { Get = fun (r : FormData) -> r.Firstname
          Set = fun v (r : FormData) -> { r with Firstname = v } }

    static member SurnameLens =
        { Get = fun (r : FormData) -> r.Surname
          Set = fun v (r : FormData) -> { r with Surname = v } }

    static member EmailLens =
        { Get = fun (r : FormData) -> r.Email
          Set = fun v (r : FormData) -> { r with Email = v } }


type Model =
    { IsLoading : bool
      FormData : FormData
      UserId : int }

    static member FormDataLens =
        { Get = fun (r : Model) -> r.FormData
          Set = fun v (r : Model) -> { r with FormData = v } }

type Msg =
    | FetchUser
    | FetchUserSuccess of Shared.Types.User
    | NetworkError of exn
    | ChangeFirstname of string
    | ChangeSurname of string
    | ChangeEmail of string
    | Cancel
    | Submit
    | EditUserResponse of obj
