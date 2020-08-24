module App

open Fable.React
open Browser.Dom
open lib.core
open pages.Home
open System.Text.RegularExpressions

ReactDom.render(div [] [
    str "Editor"
    view
], document.getElementById("root"))

let target = testsLines.[1]

let (|Regex|_|) pattern input =
    let m = Regex.Match(input, String.concat "" pattern)
    if m.Success then Some(m.Groups)
    else None

match target with
| Regex ["^"; string BuildInRegex.Word; " = "; string BuildInRegex.ValueType.String;] m ->
    console.log("var name: ", m.[1], "str value: ", m.[2])
| _ -> console.log(target, "Not a phone number")

let m = Regex "^\w+ = '.+'"
console.log(target, m, m.Match(target))
