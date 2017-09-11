module Admin.User.Edit.Types

open Okular.Lens

type Model =
    { User : Shared.Types.User option
      UserId : int }

    static member Empty =
        { User = None
          UserId = -1 }

type Msg =
    | FetchUser
    | FetchUserSuccess of Shared.Types.User
    | Error of exn
