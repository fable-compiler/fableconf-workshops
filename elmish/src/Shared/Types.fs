module Shared.Types

open Fable.Core

[<Pojo>]
type User =
    { Id : int
      Firstname: string
      Surname: string
      Email: string
      Password: string }

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
