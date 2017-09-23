module Dashboard.Types

type Model =
    { Questions : Shared.Types.Question list }

    static member Empty =
        { Questions = [] }

type Msg =
    | FetchQuestions
    | NetworkError of exn
    | FetchQuestionsSuccess of Shared.Types.Question []
