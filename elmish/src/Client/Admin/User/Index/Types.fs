module Admin.User.Index.Types

open Okular.Lens

type Model =
    { Users : Shared.Types.User list }

    static member Empty =
        { Users = [] }

    static member UsersLens =
        { Get = fun (r : Model) -> r.Users
          Set = fun v (r : Model) -> { r with Users = v } }

type Msg =
    | ChangeStr of string
    | FetchUsers
    | FetchUsersSuccess of Shared.Types.User []
    | Error of exn
    | ShowDetail of int
    | CreateUser
