module Helpers

open Fable.Core
open Fable.Import

let resolve path = Node.Exports.Path.join(Node.Globals.__dirname, path)
let combine path1 path2 = Node.Exports.Path.join(path1, path2)
