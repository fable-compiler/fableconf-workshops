[<AutoOpen>]
module Global

open Fable.Core
open Elmish

type AdminUserPage =
    | Index
    | Create
    | Edit of int

type AdminPage =
    | Index
    | User of AdminUserPage

type Page =
    | Home
    | Admin of AdminPage

let toHash page =
    match page with
    | Home -> "#home"
    | Admin admin ->
        match admin with
        | Index -> "#admin"
        | User user ->
            match user with
            | AdminUserPage.Index -> "#admin/user"
            | Create -> "#admin/user/create"
            | Edit id -> sprintf "#admin/user/%i/edit" id

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