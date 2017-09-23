module Admin.User.Create.Rest

open Fable.PowerPack
open Fable.PowerPack.Fetch
open Shared.Types

let createUser (data: Shared.Types.UserCreate) =
    promise {
        let url = serverUrl (sprintf "/user/create")
        let! res = postRecord url data []
        let! json = res.json()
        return json
    }
