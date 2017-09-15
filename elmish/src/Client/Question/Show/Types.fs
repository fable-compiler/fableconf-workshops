module Question.Show.Types

type Model =
    { Question : Shared.Types.Question option
      Answers : Shared.Types.Answer list }

    static member Empty =
        { Question =  None
          Answers = [] }

type Msg =
    | FetchDetail of int
    | NetworkError of exn
    | FetchDetailSuccess of Shared.Types.QuestionShow
