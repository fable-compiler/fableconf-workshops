[<RequireQualifiedAccess>]
module Shared.Init

open Fable.Core

type [<Pojo>] Options =
    { N: int
      boxWidth: float
      boxHeight: float }

let options =
    { N = 100
      boxWidth = 0.5
      boxHeight = 0.5 }
