module Admin.Dispatcher.Types

open Okular.Lens

type Model =
    { Index : Admin.Index.Types.Model option
      UserIndex : Admin.User.Index.Types.Model option
      UserEdit : Admin.User.Edit.Types.Model option
      UserCreate : Admin.User.Create.Types.Model option }

    static member Empty =
        { Index = None
          UserIndex = None
          UserEdit = None
          UserCreate = None }

type Msg =
    | IndexMsg of Admin.Index.Types.Msg
    | UserIndexMsg of Admin.User.Index.Types.Msg
    | UserEditMsg of Admin.User.Edit.Types.Msg
    | UserCreateMsg of Admin.User.Create.Types.Msg
