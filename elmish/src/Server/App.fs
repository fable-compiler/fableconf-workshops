module Server

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Express
open Helpers
open Database
module Validation = Shared.Validation

let app = express.Invoke()

// Configure express application
let staticOptions = createEmpty<express.``serve-static``.Options>
staticOptions.index <- Some !^"index.html"



let output = resolve ".."
let publicPath = combine output "../public"
let clientPath = combine output "client"

app
// Register the static directories
|> Express.Sugar.``use`` (express.``static``.Invoke(publicPath, staticOptions))
|> Express.Sugar.``use`` (express.``static``.Invoke(clientPath, staticOptions))
// Register logger
|> Express.Sugar.``use`` (morgan.Exports.Morgan.Invoke(morgan.Dev))
|> Express.Sugar.``use`` (bodyParser.Globals.json())
|> ignore

// Routing
// Prevent cache over JSON response
app.set("etag", false) |> ignore

app
|> Express.Sugar.get
        "/status"
        (fun req res ->
            Express.Sugar.Response.send "Server is running !" res
        )
|> Express.Sugar.post
        "/test"
        (fun req res ->
            req.body?test |> printfn "%A"
            res.``end``()
        )
|> Express.Sugar.get
        "/user/list"
        (fun req res ->
            let users =
                Database.Users
                    .value()

            res.setHeader("Content-Type", !^"application/json")
            Express.Sugar.Response.send (toJson users) res
        )
|> Express.Sugar.get
        "/user/:id"
        (fun req res ->
            let id = unbox<int> req.``params``?id
            let user =
                Database.Users
                    .find(createObj [ "Id" ==> id])
                    .value()

            res.setHeader("Content-Type", !^"application/json")
            Express.Sugar.Response.send (toJson user) res
        )
|> Express.Sugar.put
    "/user/:id/edit"
    (fun req res ->
        let id = unbox<int> req.``params``?id
        let user = unbox<Shared.Types.UserEdit> req.body

        Database.Users
            .find(createObj [ "Id" ==> id])
            .assign(user)
            .write()

        res.setHeader("Content-Type", !^"application/json")
        Express.Sugar.Response.send (toJson "success") res
    )
|> Express.Sugar.post
    "/user/create"
    (fun req res ->
        let data = unbox<Shared.Types.UserCreate> req.body

        let user : Shared.Types.User =
            { Id = Database.NextUserId
              Firstname = data.Firstname
              Surname = data.Surname
              Email = data.Email
              Password = data.Password
              Avatar = "" }

        Database.Users
            .push(user)
            .write()

        res.setHeader("Content-Type", !^"application/json")
        Express.Sugar.Response.send (toJson "success") res
    )
|> Express.Sugar.post
    "/sign-in"
    (fun req res ->
        let data = unbox<Shared.Types.SignInData> req.body

        let user =
            Database.Users
                .find(
                    createObj [
                        "Email" ==> data.Email
                        "Password" ==> data.Password
                    ]
                ).value()

        res.setHeader("Content-Type", !^"application/json")

        let result : Shared.Types.GenericJsonResponse =
            if isNull user then
                { Code = Validation.SignIn.UserNotFound
                  Data = null }
            else
                let data : Shared.Types.SignInResponse =
                    { Token = "I am a dummy token for now" }
                { Code = Validation.SignIn.SignInSuccess
                  Data = data :> obj }

        Express.Sugar.Response.send (toJson result) res
    )
|> Express.Sugar.get
    "/question/list"
    (fun req res ->
        let questions =
            Database.Questions
                .value() |> unbox<QuestionDb []>

        let questionsWithUser : Shared.Types.Question [] =
            questions
            |> Array.map Transform.generateQuestion

        res.setHeader("Content-Type", !^"application/json")
        Express.Sugar.Response.send (toJson questionsWithUser) res
    )
|> Express.Sugar.get
    "/question/:id"
    (fun req res ->
        let id = unbox<int> req.``params``?id
        let questionDb =
            Database.Questions
                .find(createObj [ "Id" ==> id])
                .value() |> unbox<QuestionDb>

        let question : Shared.Types.QuestionShow =
            { Question = Transform.generateQuestion questionDb
              Answsers =
                questionDb.Answsers
                |> Array.map Transform.generateAnswer
                |> Array.toList
            }

        res.setHeader("Content-Type", !^"application/json")
        Express.Sugar.Response.send (toJson question) res
    )
|> ignore

// Start the server
let port =
    match unbox Node.Globals.``process``.env?PORT with
    | Some x -> x
    | None -> 8080

// Live reload when in dev mode
// #if DEBUG
// let reload = importDefault<obj> "reload"
// let reloadServer = reload$(app)

// let watch = importDefault<obj> "watch"

// let watchOptions = createEmpty<Watch.Options>
// watchOptions.interval <- Some 1.

// Watch.Exports.watchTree(output, watchOptions, fun f cur prev ->
//     reloadServer?reload$() |> ignore
// )
// #endif

// let db =
Database.Lowdb
    .defaults(
        { Users =
            [| { Id = 1
                 Firstname = "Maxime"
                 Surname = "Mangel"
                 Email = "mangel.maxime@fableconf.com"
                 Password = "maxime"
                 Avatar = "maxime_mangel.png" }
               { Id = 2
                 Firstname = "Alfonso"
                 Surname = "Garciacaro"
                 Email = "garciacaro.alfonso@fableconf.com"
                 Password = "alfonso"
                 Avatar = "alfonso_garciacaro.png" }
               { Id = 3
                 Firstname = "Robin"
                 Surname = "Munn"
                 Email = "robin.munn@fableconf.com"
                 Password = "robin"
                 Avatar = "robin_munn.png" }
            |]
          Questions =
            [| { Id = 1
                 AuthorId = 3
                 Title = "What is the average wing speed of an unladen swallow?"
                 Description =
                    """
Hello, yesterday I saw a flight of swallows and was wondering what their **average wing speed** is.

If you know the answer please share it.
                    """
                 CreatedAt = "2017-09-14T17:44:28.103Z"
                 Answsers =
                    [| { Id = 1
                         AuthorId = 1
                         Content = "Aliquam rhoncus nec mi eget dictum. Praesent ut ornare est. Vivamus porttitor orci sit amet turpis laoreet fringilla. Sed viverra massa a nulla fringilla placerat. Integer luctus iaculis convallis. Donec rhoncus consectetur risus, a egestas leo finibus quis. Etiam ultrices elit felis, ac imperdiet ipsum euismod rutrum. Aliquam et dolor sapien. Sed eget laoreet ex."
                         CreatedAt = "2017-09-14T19:57:33.103Z" }
                       { Id = 1
                         AuthorId = 2
                         Content = "Proin convallis scelerisque enim, mattis malesuada diam suscipit quis. Nulla consectetur purus sit amet nisl tempor, ac efficitur augue rhoncus. "
                         CreatedAt = "2017-09-15T22:31:16.103Z" }
                    |]
               }
               { Id = 2
                 AuthorId = 1
                 Title = "Why did you create Fable ?"
                 Description =
                    """
Hello Alfonso,
I wanted to know why did you create Fable. Did you always planned to use F# ? Or was you thinking to others languages ?
                    """
                 CreatedAt = "2017-09-12T09:27:28.103Z"
                 Answsers = [||] }
            |]
        }
    ).write()
|> ignore

app.listen(port, !!(fun _ ->
    printfn "Server started: http://localhost:%i" port
))
|> ignore
