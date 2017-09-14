module SignIn.Types

open Okular.Lens

type FormData =
    { Email : StringField
      Password : StringField }

    static member EmailLens =
        { Get = fun (r : FormData) -> r.Email
          Set = fun v (r : FormData) -> { r with Email = v } }

    static member PasswordLens =
        { Get = fun (r : FormData) -> r.Password
          Set = fun v (r : FormData) -> { r with Password = v } }


type Model =
    { IsLoading : bool
      FormData : FormData }

    static member FormDataLens =
        { Get = fun (r : Model) -> r.FormData
          Set = fun v (r : Model) -> { r with FormData = v } }

type Msg =
    | ChangeEmail of string
    | ChangePassword of string
    | Submit
    | NetworkError of exn
    | SignInSuccess of Shared.Types.GenericJsonResponse
