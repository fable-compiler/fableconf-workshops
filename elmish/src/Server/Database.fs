module Database

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Helpers
open Shared.Types

[<Pojo>]
type AnswerDb =
    { Id : int
      AuthorId : int
      Content : string
      CreatedAt : string }

[<Pojo>]
type QuestionDb =
    { Id : int
      AuthorId : int
      Title : string
      Description : string
      CreatedAt : string
      Answsers : AnswerDb [] }

[<Pojo>]
type DatabaseData =
    { Users : User []
      Questions : QuestionDb [] }

let dbFile = resolve("../../ressources/db.json")
let adapter = Lowdb.FileSyncAdapter(dbFile)

let mutable ``do not use directly db`` : Lowdb.Lowdb option = Option.None

type Database =
    static member Lowdb
        with get() : Lowdb.Lowdb =
            if ``do not use directly db``.IsNone then
                ``do not use directly db`` <- Lowdb.Lowdb(adapter) |> Some

            ``do not use directly db``.Value

    static member NextUserId
        with get() : int =
            let user =
                Database.Users
                    .sortBy("Id")
                    ?last()
                    ?value()
                |> unbox<User>
            user.Id + 1

    static member Users
        with get() : Lowdb.Lowdb =
            Database.Lowdb
                .get(!^"Users")

    static member Questions
        with get() : Lowdb.Lowdb =
            Database.Lowdb
                .get(!^"Questions")
