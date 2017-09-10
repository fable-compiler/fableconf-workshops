module Shared.Types

open Fable.Core

[<Pojo>]
type Users =
    { Firstname: string
      Surname: string
      Email: string
      Password: string }
