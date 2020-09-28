module App

open Fable.React
open Browser.Dom
open pages.Home
open System.Text.RegularExpressions
open lib.core.BuildInRegex
open lib.parser.Assignment

ReactDom.render (div [] [ str "Editor"; view ], document.getElementById ("root"))

let target = testsLines.[16]

let (|Regex|_|) pattern input =
    let m =
        Regex.Match(input, String.concat "" pattern)

    if m.Success then Some(m) else None

type AST(?kind: ASTKind, ?value: string) =
    member this.Kind = kind
    member this.Value = value

let getAstKind (context: AST list) index =
    let ret = (List.tryItem index context)
    if ret.IsSome then ret.Value.Kind else None

let (?=) a b = a = Some b


type RepeatType =
    | Black
    | White

type RecognizeTimesNode = { Kind: ASTKind; Times: int }

type RecognizeRangeNode = { Kind: ASTKind; Max: int; Min: int }

type RecognizeRepeatNode =
    { Type: RepeatType
      Kind: RecognizeNode list }

and RecognizeNode =
    | TimesNode of RecognizeTimesNode
    | RangeNode of RecognizeRangeNode
    | RepeatNode of RecognizeRepeatNode

let rec recognizeOne (recoList: RecognizeNode list) (context: AST list) index =
    let find = getAstKind context
    let reco = recoList.[index]
    console.log ("reco", reco, index)

    match reco with
    | TimesNode m ->
        let findNotMatch r =
            console.log("find not match", find (index + r), m.Kind, index, r)
            not (find (index + r) ?= m.Kind)
        if m.Times > 0 then
            let notMatch = List.tryFindIndex findNotMatch [ 0 ]
            notMatch.IsSome
        else
            false
    | RangeNode m ->
        let findNotMatch r =
            console.log("find not match", find (index + r), m.Kind, index, r)
            not (find (index + r) ?= m.Kind)
        let notMatchMin =
            List.tryFindIndex findNotMatch [ 0 .. m.Min - 1 ]
        if notMatchMin.IsSome then false
        else
            let notMatchMax = List.tryFindIndex findNotMatch [ m.Min .. m.Max - 1 ]
            notMatchMax.IsSome
    | RepeatNode m -> recognize m.Kind context.[index..]

and recognize (recoList: RecognizeNode list) (context: AST list) =
    let notMatch =
        List.tryFindIndex (recognizeOne recoList context) [ 0 .. recoList.Length - 1 ]
    notMatch.IsSome

let testReco: RecognizeNode list =
    [ TimesNode { Kind = Operator Arrow; Times = 1 }
      TimesNode { Kind = RightParentheses; Times = 1 }
      RangeNode { Kind = Word; Max = 1; Min = 0 }
      RepeatNode
          { Type = White
            Kind =
                [ TimesNode { Kind = Operator Comma; Times = 1 }
                  TimesNode { Kind = Word; Times = 1 } ] }
      TimesNode { Kind = LeftParentheses; Times = 1 }
      TimesNode { Kind = Word; Times = 1 } ]

//let guessDeclareFunction (context: AST list) =
//    let find = getAstKind context
//    find 0
//    ?= Operator Arrow
//    && find 1 ?= RightParentheses
//    && find 2 ?= Word
//    && let mutable i = 3
//       while find i ?= Word || find i ?= Operator Comma do
//           i <- i + 1
//       find i ?= LeftParentheses && find (i + 1) ?= Word

let rec eval (context: AST list) (target: string) =
    let evalx kind (progma: Match) =
        let catched = AST(kind, string progma.Value)
        console.log ("catched", progma.Value, target, Array.ofList context)
        //        console.log ("is declare function ?", guessDeclareFunction context)
        [ if progma.Value.Length < target.Length then
            let next =
                target.[progma.Value.Length + progma.Index..]

            yield! eval (catched :: context) next
          catched ]

    match target with
    | Regex [ "^"; BuildInRegexs.Word ] m ->
        console.log ("is Word", m)
        evalx Word m
    | Regex [ "^"; BuildInRegexs.LeftParentheses ] m ->
        console.log ("is LeftParentheses", m)
        evalx LeftParentheses m
    | Regex [ "^"; BuildInRegexs.RightParentheses ] m ->
        console.log ("is RightParentheses", m)
        evalx RightParentheses m
    | Regex [ "^"; BuildInRegexs.KeyWord.If ] m ->
        console.log ("is KeyWord If", m)
        evalx (KeyWord If) m
    | Regex [ "^"; BuildInRegexs.KeyWord.For ] m ->
        console.log ("is KeyWord For", m)
        evalx (KeyWord For) m
    | Regex [ "^"; BuildInRegexs.Operator.Comma ] m ->
        console.log ("is Operator Comma", m)
        evalx (Operator Comma) m
    | Regex [ "^"; BuildInRegexs.Operator.Arrow ] m ->
        console.log ("is Operator Arrow", m)
        evalx (Operator Arrow) m
    | Regex [ "^"; BuildInRegexs.Operator.Assignment ] m ->
        console.log ("is Operator Assignment", m)
        evalx (Operator Assignment) m
    | Regex [ "^"; BuildInRegexs.ValueType.String ] m ->
        console.log ("is string", m)
        evalx (ValueType String) m
    | Regex [ "^"; BuildInRegexs.ValueType.Number ] m ->
        console.log ("is Number", m)
        evalx (ValueType Number) m
    | Regex [ "^"; BuildInRegexs.ValueType.Boolean ] m ->
        console.log ("is Boolean", m)
        evalx (ValueType Boolean) m
    | _ ->
        let restTarget = [| for ch in target -> int ch |]
        console.log (target, "Not Match", restTarget)
        [ for template in rest do
            let m = Regex.Match(target, template.Matcher)
            console.log ("is rest", m, "|" + target + "|", template)
            if m.Success && m.Index = 0 then yield! evalx template.Kind m ]

for line in testsLines.[16..17] do
    let context = eval [] line
    let judge = recognize testReco context
    console.log("match sth", judge)
    console.log ("eval ast", context |> List.toArray)

type ASTCollector = { Source: string; Ast: AST [] }

type Context =
    { Type: string
      Scope: Context []
      Elements: AST [] }
