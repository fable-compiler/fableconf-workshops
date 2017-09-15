module Shared.Types

open Fable.Core

[<Pojo>]
type User =
    { Id : int
      Firstname: string
      Surname: string
      Email: string
      Password: string
      Avatar : string }

[<Pojo>]
type UserEdit =
    { Firstname: string
      Surname: string
      Email: string }

[<Pojo>]
type UserCreate =
    { Firstname: string
      Surname: string
      Email: string
      Password : string
      PasswordConfirmation : string }

[<Pojo>]
type SignInData =
    { Email: string
      Password : string }

[<Pojo>]
type SignInResponse =
    { Token : string }

[<Pojo>]
type GenericJsonResponse =
    { Code : string
      Data : obj }

[<Pojo>]
type UserInfo =
    { Id : int
      Firstname : string
      Surname : string
      Email : string
      Avatar : string }

[<Pojo>]
type Question =
    { Id : int
      Author : UserInfo
      Title : string
      Description : string
      CreatedAt : string }

[<Pojo>]
type Answer =
    { Id : int
      Author : UserInfo
      Content : string
      CreatedAt : string }

[<Pojo>]
type AnswerCreate =
    { AuthorId : int
      Content : string }

[<Pojo>]
type QuestionShow =
    { Question : Question
      Answsers : Answer list }
