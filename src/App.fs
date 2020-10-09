module App

open Fable.React
open Browser.Dom
open lib.parser.Behavior
open pages.Home
open System.Text.RegularExpressions
open lib.core.BuildInRegex

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

(**
识别出一个模式节点是否匹配一串AST，返回匹配到的位置索引
*)

let rec recognizeOne (recoNode: RecognizeNode) (context: AST list) =
  let find = getAstKind context
//  console.log ("reco one", recoNode, List.toArray context)

  match recoNode with
  | TimesNode m ->
      let findNotMatch r =
        //            console.log("find not match", find (index + r), m.Kind, index, r)
        not (find r ?= m.Kind)

      if m.Times > 0 then
        let notMatch =
          List.tryFindIndex findNotMatch [ 0 .. m.Times - 1 ]

        if notMatch.IsNone then Some m.Times else None
      else
        None
  | RangeNode m ->
      let findNotMatch r = not (find r ?= m.Kind)

      let notMatchMin =
        List.tryFindIndex findNotMatch [ 0 .. m.Min - 1 ]

      //      console.log("range node min", m, notMatchMin)

      if notMatchMin.IsSome then
        None
      else
        let notMatchMax =
          List.tryFindIndex findNotMatch [ m.Min .. m.Max - 1 ]
        //        console.log("range node max", m, notMatchMax)

        if notMatchMax.IsSome then notMatchMax else Some m.Max
  | RepeatNode m ->
      let kind = m.Kind()
      let mutable ret = recognize kind context
      let mutable step = ret

      //      let mutable ret =
//        recognize m.Kind context.[step..step + len]

      //      while ret.IsNone do
//      step <- step + m.Kind.Length
      while ret < context.Length && step <> 0 do
        step <- recognize kind context.[ret..]
        ret <- ret + step
//        console.log("repeat", ret, step)
      Some ret

and recognize (recoList: RecognizeNode list) (context: AST list) =
  let mutable cursor = 0

  let matchEach i =
//    console.log("cursor", cursor)
    let ret =
      recognizeOne recoList.[i] context.[cursor..]

    if ret.IsSome then
      //      console.log("ret.Value", ret.Value)
      cursor <- ret.Value + cursor
    ret.IsNone

  let notMatch =
    List.tryFindIndex matchEach [ 0 .. recoList.Length - 1 ]

//  console.log ("not match", List.toArray context, List.toArray recoList, notMatch, cursor)
  cursor


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
//    console.log ("catched", catched, progma.Value, target, Array.ofList context, progma.Value.Length < target.Length)
    //        console.log ("is declare function ?", guessDeclareFunction context)
    [ if progma.Value.Length < target.Length then
        let next =
          target.[progma.Value.Length + progma.Index..]

        yield! eval (catched :: context) next
      catched ]

  match target with
  //  | Regex [ "^"; BuildInRegexs.ValueType.String ] m ->
//      evalx (ValueType String) m
//  | Regex [ "^"; BuildInRegexs.ValueType.Number ] m ->
//      evalx (ValueType Number) m
//  | Regex [ "^"; BuildInRegexs.ValueType.Boolean ] m ->
//      evalx (ValueType Boolean) m
//  | Regex [ "^"; BuildInRegexs.Word ] m ->
//      evalx Word m
  | Regex [ "^"; BuildInRegexs.LeftParentheses ] m ->
      //        console.log ("is LeftParentheses", m)
      evalx LeftParentheses m
  | Regex [ "^"; BuildInRegexs.RightParentheses ] m ->
      //        console.log ("is RightParentheses", m)
      evalx RightParentheses m
  | Regex [ "^"; BuildInRegexs.KeyWord.If ] m ->
      //        console.log ("is KeyWord If", m)
      evalx (KeyWord If) m
  | Regex [ "^"; BuildInRegexs.KeyWord.For ] m ->
      //        console.log ("is KeyWord For", m)
      evalx (KeyWord For) m
  | Regex [ "^"; BuildInRegexs.Operator.Comma ] m ->
      //        console.log ("is Operator Comma", m)
      evalx (Operator Comma) m
  | Regex [ "^"; BuildInRegexs.Operator.Arrow ] m ->
      //        console.log ("is Operator Arrow", m)
      evalx (Operator Arrow) m
  //  | Regex [ "^"; BuildInRegexs.Operator.Assignment ] m ->
//      evalx (Operator Assignment) m
  | _ ->
      let restTarget = [| for ch in target -> int ch |]
      //      console.log (target, "into rest recognize", restTarget)
      let mutable matched = false
      [ for template in rest do
          if not matched then
            let m = Regex.Match(target, template.Matcher)
            //            console.log ("is rest", m, "|" + target + "|", template)
            if m.Success && m.Index = 0 then
              matched <- true
              yield! evalx template.Kind m ]

let isNextLine (ast: AST) =
  ast.Kind ?= Indentation NextLine

let guessBehavior (ctx: AST list) =
  let guess (behavior: Behavior) =
//    console.log("guessing", List.toArray ctx, "is", behavior.Name)
    if ctx.Length = 0 then
      false
    else
      let fin = recognize behavior.Kind ctx
      fin = ctx.Length

  List.tryFindIndex guess behaviorSet

//let context2 = eval [] testsLines.[1]
//console.log ("eval string assignment", context2 |> List.toArray)
//let judge2 = recognize isAssignString context2
//console.log ("match isAssignString", judge2)
//
//let context4 = eval [] testsLines.[2]
//console.log ("eval number assignment", context4 |> List.toArray)
//let judge4 = recognize isAssignNumber context4
//console.log ("match isAssignNumber", judge4)
//
//let context = eval [] testsLines.[16]
//console.log ("eval function", context |> List.toArray)
//let judge = recognize isFunctionDeclare context
//console.log ("match isFunctionDeclare", judge)

let mutable stack: AST list = []

type Scope = {
  Name: string
}

for (i, line) in seq { for i in 0 .. 12 -> (i, testsLines.[i]) } do
  let ctx = eval stack line
  let expression = ctx @ stack
  let guessResult = guessBehavior expression
  if (guessResult.IsNone && ctx.Length <> 0) then
    stack <- AST(Indentation NextLine, "next line")::expression
  elif guessResult.IsSome then
    stack <- []
    let behavior = behaviorSet.[guessResult.Value]
    match guessResult with
    | Some 5 ->
      console.log({| Key = expression.[2].Value; Value = expression.[0].Value|})
    | _ ->
      console.log(behavior.Name)
  console.log (i, expression |> List.toArray)

type ASTCollector = { Source: string; Ast: AST [] }

type Context =
  { Type: string
    Scope: Context []
    Elements: AST [] }
