module App

open Fable.React
open Browser.Dom
open pages.Home
open System.Text.RegularExpressions
open lib.core.BuildInRegex
open lib.parser.Assignment

ReactDom.render
    (div []
         [ str "Editor"
           view ], document.getElementById ("root"))

let target = testsLines.[1]

let (|Regex|_|) pattern input =
    let m = Regex.Match(input, String.concat "" pattern)
    if m.Success then Some(m.Groups) else None

match target with
| Regex isAssignment m ->
    console.log ("var name: ", m)
| _ -> console.log (target, "Not a phone number")

let m =
    String.concat "" isAssignment
    |> Regex

console.log (target, m, m.Match(target))

type AST =
    { Kind: string
      Name: string }
type ASTCollector = {
  source: string
  ast: AST[]
}
type Context =
    { Type: string
      Scope: Context[]
      Elements: AST[] }
