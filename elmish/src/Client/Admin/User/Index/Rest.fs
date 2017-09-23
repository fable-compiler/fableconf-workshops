module Admin.User.Index.Rest

open Fable.PowerPack
open Fable.PowerPack.Fetch
open Shared.Types

let getUserList _ =
    promise {
        let url = serverUrl "/user/list"
        let! res = fetchAs<Shared.Types.User []> url []
        return res
    }
