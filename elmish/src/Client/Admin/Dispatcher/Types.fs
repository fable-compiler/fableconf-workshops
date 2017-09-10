module Admin.Dispatcher.Types

open Lenses

type Model =
    { Index : Admin.Index.Types.Model
      UserIndex : Admin.User.Index.Types.Model }

    static member IndexLens =
        { Get = fun (r : Model) -> r.Index
          Set = fun v (r : Model) -> { r with Index = v } }

    static member UserIndexLens =
        { Get = fun (r : Model) -> r.UserIndex
          Set = fun v (r : Model) -> { r with UserIndex = v } }

type Msg =
    | IndexMsg of Admin.Index.Types.Msg
    | UserIndexMsg of Admin.User.Index.Types.Msg
