module Transform

open Fable.Core.JsInterop
open Database

let generateQuestion (questionDb : QuestionDb) : Shared.Types.Question =
    let author =
        Database.Users
            .find(createObj [ "Id" ==> questionDb.AuthorId ])
            .value() |> unbox<Shared.Types.User>

    { Id = questionDb.Id
      Author =
        { Id = author.Id
          Firstname = author.Firstname
          Surname = author.Surname
          Email = author.Email
          Avatar = author.Avatar
          Permissions = author.Permissions }
      Title = questionDb.Title
      Description = questionDb.Description
      CreatedAt = questionDb.CreatedAt }

let generateAnswer (answerDb : AnswerDb) : Shared.Types.Answer =
    let author =
        Database.Users
            .find(createObj [ "Id" ==> answerDb.AuthorId ])
            .value() |> unbox<Shared.Types.User>

    { Id = answerDb.Id
      Author =
        { Id = author.Id
          Firstname = author.Firstname
          Surname = author.Surname
          Email = author.Email
          Avatar = author.Avatar
          Permissions = author.Permissions }
      Content = answerDb.Content
      CreatedAt = answerDb.CreatedAt }

let generateUser (userDb : Shared.Types.User) : Shared.Types.UserInfo =
    { Id = userDb.Id
      Firstname = userDb.Firstname
      Surname = userDb.Surname
      Email = userDb.Email
      Avatar = userDb.Avatar
      Permissions = userDb.Permissions }
