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
              Avatar = ""
              Permissions = "" }

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
                    { Token = "I am a dummy token for now"
                      User = Transform.generateUser (unbox user) }
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
                Database.Answers
                    .filter(createObj [ "QuestionId" ==> questionDb.Id ])
                    .value()
                |> unbox<AnswerDb []>
                |> Array.map Transform.generateAnswer
                |> Array.toList
            }

        res.setHeader("Content-Type", !^"application/json")
        Express.Sugar.Response.send (toJson question) res
    )
|> Express.Sugar.post
    "/question/:id/answer"
    (fun req res ->
        let questionId = unbox<int> req.``params``?id

        let data = unbox<Shared.Types.CreateAnswer> req.body

        let question =
            Database.Questions
                .find(createObj [ "Id" ==> questionId])
                .value()

        let result : Shared.Types.GenericJsonResponse =
            if isNull question then
                { Code = Validation.Question.QuestionNotFound
                  Data = null }
            else

                let answer : AnswerDb =
                    { Id = Database.NextAnswerId
                      QuestionId = questionId
                      AuthorId = data.AuthorId
                      Content = data.Content
                      CreatedAt = System.DateTime.UtcNow.ToString().Replace("\"", "")  }

                Database.Answers
                    .push(answer)
                    .write()

                let data : Shared.Types.Answer = Transform.generateAnswer answer

                { Code = Validation.Question.Show.CreateAnswerSuccess
                  Data = data :> obj }

        res.setHeader("Content-Type", !^"application/json")
        Express.Sugar.Response.send (toJson result) res
    )
|> ignore

// Start the server
let port =
    match unbox Node.Globals.``process``.env?PORT with
    | Some x -> x
    | None -> 8080

//Live reload when in dev mode
#if DEBUG
let reload = importDefault<obj> "reload"
let reloadServer = reload$(app)

let watch = importDefault<obj> "watch"

let watchOptions = createEmpty<Watch.Options>
watchOptions.interval <- Some 1.

Watch.Exports.watchTree(output, watchOptions, fun f cur prev ->
    reloadServer?reload$() |> ignore
)
#endif

// let db =
Database.Lowdb
    .defaults(
        { Users =
            [| { Id = 1
                 Firstname = "Maxime"
                 Surname = "Mangel"
                 Email = "mangel.maxime@fableconf.com"
                 Password = "maxime"
                 Avatar = "maxime_mangel.png"
                 Permissions = "admin" }
               { Id = 2
                 Firstname = "Alfonso"
                 Surname = "Garciacaro"
                 Email = "garciacaro.alfonso@fableconf.com"
                 Password = "alfonso"
                 Avatar = "alfonso_garciacaro.png"
                 Permissions = "" }
               { Id = 3
                 Firstname = "Robin"
                 Surname = "Munn"
                 Email = "robin.munn@fableconf.com"
                 Password = "robin"
                 Avatar = "robin_munn.png"
                 Permissions = "" }
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
                 CreatedAt = "2017-09-14T17:44:28.103Z" }
               { Id = 2
                 AuthorId = 1
                 Title = "Why did you create Fable ?"
                 Description =
                    """
Hello Alfonso,
I wanted to know why did you create Fable. Did you always planned to use F# ? Or was you thinking to others languages ?
                    """
                 CreatedAt = "2017-09-12T09:27:28.103Z" }
            |]
          Answers =
            [| { Id = 1
                 QuestionId = 1
                 AuthorId = 1
                 Content =
                    """
> What do you mean, an African or European Swallow ?
>
> Monty Python’s: The Holy Grail

Ok I must admit, I use google to search the question and found a post explaining the reference :).

I thought you was asking it seriously well done.
                    """
                 CreatedAt = "2017-09-14T19:57:33.103Z" }
               { Id = 2
                 QuestionId = 1
                 AuthorId = 2
                 Content =
                    """
Maxime,

I believe you found [this blog post](http://www.saratoga.com/how-should-i-know/2013/07/what-is-the-average-air-speed-velocity-of-a-laden-swallow/).

And so Robin, the conclusion of the post is:

> In the end, it’s concluded that the airspeed velocity of a (European) unladen swallow is about 24 miles per hour or 11 meters per second.
                    """
                 CreatedAt = "2017-09-15T22:31:16.103Z" }
            |]
        }
    ).write()
|> ignore

app.listen(port, !!(fun _ ->
    printfn "Server started: http://localhost:%i" port
))
|> ignore
