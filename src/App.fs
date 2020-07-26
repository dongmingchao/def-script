module App

open Fable.React
open Browser.Dom
open pages.Home

ReactDom.render(div [] [
    str "Editor"
    view
], document.getElementById("root"))
