module Question.Show.State

open Elmish
open Types
open Okular
open Okular.Operators
module Validation = Shared.Validation.Question

let init id  =
    Model.Empty, Cmd.ofMsg (FetchDetail id)

let update msg model =
    match msg with
    | NetworkError error ->
        printfn "[Question.Show.State][Network error] %s" error.Message
        model, Cmd.none

    | FetchDetail id ->
        model, Cmd.ofPromise Rest.getDetails id FetchDetailSuccess NetworkError

    | FetchDetailSuccess result ->
        { model with Question = Some result.Question
                     Answers = result.Answsers }, Cmd.none

    | ChangeReply value ->
        model
        |> Lens.set (Model.ReplyLens >-> StringField.ValueLens) value
        |> Lens.set (Model.ReplyLens >-> StringField.ErrorLens) None, Cmd.none

    | Submit ->
        if model.IsWaitingReply then
            model, Cmd.none
        else
            let createAnswerData : Shared.Types.CreateAnswer =
                { AuthorId = LocalStorage.Session.User.Id
                  Content = model.Reply.Value }

            if Validation.Show.verifyCreateAnswer createAnswerData then
                { model with IsWaitingReply = true }, Cmd.ofPromise Rest.createAnswer (model.Question.Value.Id, createAnswerData) CreateAnswerSuccess NetworkError
            else
                model
                |> Lens.set (Model.ReplyLens >-> StringField.ErrorLens) (Validation.Show.verifyAnswer model.Reply.Value), Cmd.none

    | CreateAnswerSuccess result ->
        match result.Code with
        | Validation.QuestionNotFound ->
            { model with IsWaitingReply = false }, Cmd.none

        | Validation.Show.CreateAnswerSuccess ->
            { model with IsWaitingReply = false
                         Answers = model.Answers @ [ result.Data :?> Shared.Types.Answer ]
                         Reply = StringField.Empty  }, Cmd.none

        | code ->
            Logger.errorFn "Unkown code: %O" code
            { model with IsWaitingReply = false }, Cmd.none
