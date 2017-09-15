module App.Types

open Okular.Lens

type Msg =
    | AdminMsg of Admin.Dispatcher.Types.Msg
    | DashboardMsg of Dashboard.Types.Msg
    | SignInMsg of SignIn.Types.Msg
    | QuestionMsg of Question.Show.Types.Msg

type Model =
    { CurrentPage : Page
      AdminModel : Admin.Dispatcher.Types.Model
      Dashboard : Dashboard.Types.Model
      QuestionModel : Question.Show.Types.Model
      SignIn : SignIn.Types.Model
      Session : Session option }

    static member AdminModelLens =
        { Get = fun (r : Model) -> r.AdminModel
          Set = fun v (r : Model) -> { r with AdminModel = v } }
