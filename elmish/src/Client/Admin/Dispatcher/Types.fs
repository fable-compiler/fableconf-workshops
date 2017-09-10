module Admin.Dispatcher.Types

type Model =
    { Index : Admin.Index.Types.Model }

type Msg =
| IndexMsg of Admin.Index.Types.Msg
