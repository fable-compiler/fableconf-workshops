module Admin.User.Index.Types

open Lenses

type Model =
    { Users : Shared.Types.Users list }

    static member Empty =
        { Users = [] }

    static member UsersLens =
        { Get = fun (r : Model) -> r.Users
          Set = fun v (r : Model) -> { r with Users = v } }

type Msg =
    | ChangeStr of string
    | FetchUsers
