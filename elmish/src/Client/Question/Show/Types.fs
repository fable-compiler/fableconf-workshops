module Question.Show.Types

open Okular.Lens

type Model =
    { Question : Shared.Types.Question option }

    static member Empty =
        { Question =  None }

type Msg =
    | None
