[<AutoOpen>]
module Global

open Fable.Core
open Fable.Import
open Elmish

type AdminUserPage =
    | Index
    | Create
    | Edit of int

type AdminPage =
    | Index
    | User of AdminUserPage

type AuthenticatedPage =
    | Dashboard
    | Admin of AdminPage

type SessionAction =
    | ReloadToken of string option

type Page =
    | SignIn
    | AuthPage of AuthenticatedPage
    | Session of SessionAction

let toHash page =
    match page with
    | Session session ->
        match session with
        | ReloadToken nextUrl ->
            match nextUrl with
            | Some url -> "#session?nextUrl=" + url
            | None -> "#session?nextUrl="
    | AuthPage authPage ->
        match authPage with
        | Dashboard -> "#dashboard"
        | Admin admin ->
            match admin with
            | Index -> "#admin"
            | User user ->
                match user with
                | AdminUserPage.Index -> "#admin/user"
                | Create -> "#admin/user/create"
                | Edit id -> sprintf "#admin/user/%i/edit" id
    | SignIn -> "#sign-in"

let serverUrl path = "http://localhost:8080" + path

let secureUpdate (update: 'b -> 'a -> 'a * Cmd<'b>) (msg: 'b) (optionalModel : 'a option) =
    if optionalModel.IsSome then
        update msg optionalModel.Value
    else
        failwith "Optional model has no value"

let secureView (view: 'a -> ('b -> unit) -> Fable.Import.React.ReactElement) (optionalModel : 'a option) =
    if optionalModel.IsSome then
        view optionalModel.Value
    else
        failwith "Optional model has no value"

type Session =
    { Token : string }

// type References =
//     static member Map
//         with get() = unbox<L.Map> Browser.window?map
//         and set(value : L.Map) = Browser.window?map <- value

type LocalStorage =
    static member Token
        with get() = Browser.localStorage.getItem("session_token") :?> string
        and set(value) = Browser.localStorage.setItem("session_token", value)

    static member DestroyToken () =
        Browser.localStorage.removeItem("session_token")

open Okular.Lens

type StringField =
    { Value : string
      Error : string option }

    static member Empty =
        { Value = ""
          Error = None }

    static member Initial value =
        { Value = value
          Error = None }

    static member ValueLens =
        { Get = fun (r : StringField) -> r.Value
          Set = fun v (r : StringField) -> { r with Value = v } }

    static member ErrorLens =
        { Get = fun (r : StringField) -> r.Error
          Set = fun v (r : StringField) -> { r with Error = v } }
