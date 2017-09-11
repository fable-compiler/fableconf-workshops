module Server

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Express
open Helpers
open Database

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
                Database.Lowdb
                    .get(!^"Users")
                    .value()

            res.setHeader("Content-Type", !^"application/json")
            Express.Sugar.Response.send (toJson users) res
        )
// |> Express.get
//         ""
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
                 Password = "maxime" }
               { Id = 2
                 Firstname = "Alfonso"
                 Surname = "Garciacaro"
                 Email = "garciacaro.alfonso@fableconf.com"
                 Password = "alfonso" }
            |]
        }
    ).write()
|> ignore

app.listen(port, !!(fun _ ->
    printfn "Server started: http://localhost:%i" port
))
|> ignore