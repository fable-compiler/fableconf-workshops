module Admin.Dispatcher.Types

type Model =
    { Index : Admin.Index.Types.Model
      UserIndex : Admin.User.Index.Types.Model }

type Msg =
    | IndexMsg of Admin.Index.Types.Msg
    | UserIndexMsg of Admin.User.Index.Types.Msg
