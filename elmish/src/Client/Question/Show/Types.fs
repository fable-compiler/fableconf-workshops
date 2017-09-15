module Question.Show.Types

open Okular.Lens

type Model =
    { Question : Shared.Types.Question option
      Answers : Shared.Types.Answer list
      Reply : StringField
      IsWaitingReply : bool }

    static member Empty =
        { Question =  None
          Answers = []
          Reply = StringField.Empty
          IsWaitingReply = false }

    static member ReplyLens =
        { Get = fun (r : Model) -> r.Reply
          Set = fun v (r : Model) -> { r with Reply = v } }

type Msg =
    | FetchDetail of int
    | NetworkError of exn
    | FetchDetailSuccess of Shared.Types.QuestionShow
    | ChangeReply of string
    | Submit
    | CreateAnswerSuccess of Shared.Types.GenericJsonResponse
