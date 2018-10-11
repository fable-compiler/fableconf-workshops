module Client

open System

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import

open Canvas

open Shared

let matter: Matter.IExports = importAll "matter-js"

type Model = int

type Msg =
    | Tick of delta : float

let init () = 0

let update (model: Model) = function
    | Tick _ -> model

let view (model : Model) (ctx: Context) _ =
    ctx.restore()

let subscribe (canvas: Browser.HTMLCanvasElement) dispatch (model : Model) =
    ()

Canvas.Start("canvas", init(), Tick, update, view, subscribe)