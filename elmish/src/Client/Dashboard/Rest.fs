module Dashboard.Rest

open Fable.PowerPack
open Fable.PowerPack.Fetch

let getQuestions id =
    promise {
        let url = serverUrl "/question/list"
        let! res = fetchAs<Shared.Types.Question []> url []
        return res
    }
