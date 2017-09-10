module App.Types

type Msg =
    | AdminMsg of Admin.Dispatcher.Types.Msg
    | HomeMsg of Home.Types.Msg

type Model =
    { CurrentPage : Page
      AdminModel : Admin.Dispatcher.Types.Model
      Home: Home.Types.Model }
