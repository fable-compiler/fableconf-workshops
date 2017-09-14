module SignIn.Rest

open Fable.PowerPack
open Fable.PowerPack.Fetch
open Fable.Core.JsInterop
open Shared.Types

let signIn (data : Shared.Types.SignInData) =
    promise {
        let url = serverUrl "/sign-in"
        let! res = postRecord url data []
        let! txt = res.text()

        do! Promise.sleep 500
        return ofJson<GenericJsonResponse> txt
    }
