module Question.Show.Rest

open Fable.PowerPack
open Fable.PowerPack.Fetch
open Fable.Core.JsInterop
open Shared.Types

let getDetails id =
    promise {
        let url = serverUrl (sprintf "/question/%i" id)
        let! res = fetchAs<QuestionShow> url []
        return res
    }

let createAnswer (questionId, createAnswer : CreateAnswer) =
    promise {
        let url = serverUrl (sprintf "/question/%i/answer" questionId)
        let! res = postRecord url createAnswer []
        let! txt = res.text()

        do! Promise.sleep 500
        return ofJson<GenericJsonResponse> txt
    }
