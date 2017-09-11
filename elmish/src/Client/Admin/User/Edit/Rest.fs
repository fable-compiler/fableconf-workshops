module Admin.User.Edit.Rest

open Fable.PowerPack
open Fable.PowerPack.Fetch
open Shared.Types

let getUser id =
    promise {
        let url = serverUrl (sprintf "/user/%i" id)
        let! res = fetchAs<Shared.Types.User> url []
        return res
    }
