module App

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Recharts
open Fable.Recharts.Props
module SVG = Fable.Helpers.React.Props

let data: Array = importDefault "../../build/test.json"

let barChart(data) =
    let margin = { top=5.; bottom=5.; right=20.; left=0. }
    barChart [Width 600.; Height 300.; Margin margin; Data data] [
        xaxis [DataKey "name"]
        yaxis []
        tooltip []
        legend []
        cartesianGrid [SVG.StrokeDasharray "3 3"]
        bar [DataKey "value"; SVG.Fill "#8884d8"]
    ]

let renderApp() =
    let reactEl = barChart data
    let domEl = Browser.document.getElementById("react-container")
    ReactDom.render(reactEl, domEl)

renderApp()