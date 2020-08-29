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

let target = testsLines.[16]

let (|Regex|_|) pattern input =
    let m = Regex.Match(input, String.concat "" pattern)
    if m.Success then Some(m) else None

type AST(?kind: ASTKind, ?value: string) =
  member this.Kind = kind
  member this.Value = value

let rec eval (target: string) = 
  let evalx kind (progma: Match) =
    [
      if progma.Value.Length + progma.Index < target.Length then
        let next = target.[progma.Value.Length + progma.Index..]
        yield! eval next
      AST(kind, string progma.Value)
    ]
  match target with
  | Regex ["^"; BuildInRegexs.Word] m ->
      console.log("is Word", m)
      evalx Word m
  | Regex ["^"; BuildInRegexs.LeftParentheses] m ->
      console.log("is LeftParentheses", m)
      evalx LeftParentheses m
  | Regex ["^"; BuildInRegexs.RightParentheses] m ->
      console.log("is RightParentheses", m)
      evalx RightParentheses m
  | Regex ["^"; BuildInRegexs.KeyWord.If] m ->
      console.log("is KeyWord If", m)
      evalx (KeyWord If) m
  | Regex ["^"; BuildInRegexs.KeyWord.For] m ->
      console.log("is KeyWord For", m)
      evalx (KeyWord For) m
  | Regex ["^"; BuildInRegexs.Operator.Comma] m ->
      console.log("is Operator Comma", m)
      evalx (Operator Comma) m
  | Regex ["^"; BuildInRegexs.Operator.Assignment] m ->
      console.log("is Operator Assignment", m)
      evalx (Operator Assignment) m
  | Regex ["^"; BuildInRegexs.Operator.Arrow] m ->
      console.log("is Operator Arrow", m)
      evalx (Operator Arrow) m
  | Regex ["^"; BuildInRegexs.ValueType.String] m ->
      console.log("is string", m)
      evalx (ValueType String) m
  | Regex ["^"; BuildInRegexs.ValueType.Number] m ->
      console.log("is Number", m)
      evalx (ValueType Number) m
  | Regex ["^"; BuildInRegexs.ValueType.Boolean] m ->
      console.log("is Boolean", m)
      evalx (ValueType Boolean) m
  | _ ->
      console.log (target, "Not Match")
      []

for ast in eval target do
  console.log("eval ast", ast)

type ASTCollector = {
  Source: string
  Ast: AST[]
}
type Context =
    { Type: string
      Scope: Context[]
      Elements: AST[] }
