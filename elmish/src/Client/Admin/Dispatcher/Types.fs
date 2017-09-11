module Admin.Dispatcher.Types

open Okular.Lens

type Model =
    { Index : Admin.Index.Types.Model option
      UserIndex : Admin.User.Index.Types.Model option
      UserEdit : Admin.User.Edit.Types.Model option }

    static member IndexLens =
        { Get = fun (r : Model) -> r.Index
          Set = fun v (r : Model) -> { r with Index = v } }

    static member UserEditLens =
        { Get = fun (r : Model) -> r.UserEdit
          Set = fun v (r : Model) -> { r with UserEdit = v } }

    static member UserIndexLens =
        { Get = fun (r : Model) -> r.UserIndex
          Set = fun v (r : Model) -> { r with UserIndex = v } }

    static member Empty =
        { Index = None
          UserIndex = None
          UserEdit = None }

type Msg =
    | IndexMsg of Admin.Index.Types.Msg
    | UserIndexMsg of Admin.User.Index.Types.Msg
    | UserEditMsg of Admin.User.Edit.Types.Msg