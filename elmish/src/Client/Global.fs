module Global

open Fable.Core

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
            | Edit id -> sprintf "#admin/user/%i" id
