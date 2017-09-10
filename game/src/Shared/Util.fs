module Shared.Util

open Fable.Core
open Fable.Import

// Don't make this directly public to preventç
// double evaluation of arguments
[<Emit("$1.postMessage($0, [$0.buffer])")>]
let private transferArrayJs (ar: 'T[]) (worker: Browser.Worker): unit = jsNative

let transferArray (ar: 'T[]) (worker: Browser.Worker): unit =
    transferArrayJs ar worker
