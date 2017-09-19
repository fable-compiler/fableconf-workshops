module App

open Fable.Core
open Fable.Import

let renderApp() =
    let reactEl = Examples.barChartSample()
    let domEl = Browser.document.getElementById("react-container")
    ReactDom.render(reactEl, domEl)

renderApp()