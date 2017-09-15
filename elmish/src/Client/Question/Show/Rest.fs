module Question.Show.Rest

open Fable.PowerPack
open Fable.PowerPack.Fetch

let getDetails id =
    promise {
        let url = serverUrl (sprintf "/question/%i" id)
        let! res = fetchAs<Shared.Types.QuestionShow> url []
        return res
    }
