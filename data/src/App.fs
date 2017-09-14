module FableConf_Data

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Recharts
open Fable.Recharts.Props
module SVG = Fable.Helpers.React.Props
// module R = Fable.Helpers.React

type [<Pojo>] MyData =
    { name: string
      uv: float }

let data =
    [| { name = "Page A"; uv = 400. }
       { name = "Page B"; uv = 300. }
       { name = "Page C"; uv = 300. }
       { name = "Page D"; uv = 200. }
       { name = "Page E"; uv = 278. }
       { name = "Page F"; uv = 189. } |]

let view() =
    lineChart [
        Width 600.
        Height 300.
        Data data
        Margin { top = 5.
                 bottom = 5.
                 right = 20.
                 left = 0. }
    ] [
        line [
            Type Monotone
            DataKey "uv"
            SVG.Stroke "#8884d8"
            SVG.StrokeWidth 2.
        ]
        cartesianGrid [
            SVG.Stroke "#ccc"
            SVG.StrokeDasharray "5 5"
        ]
        xaxis [DataKey "name"]
        yaxis []
        tooltip []
    ]

let renderApp() =
    let reactEl = view()
    let domEl = Browser.document.getElementById("react-container")
    ReactDom.render(reactEl, domEl)

renderApp()