module App

open System.Collections.Generic
open Fable.React
open Browser.Dom
open lib.parser.Behavior
open pages.Home
open System.Text.RegularExpressions
open lib.core.BuildInRegex

ReactDom.render (div [] [ str "Editor"; view ], document.getElementById "root")

let target = testsLines.[16]

let (|Regex|_|) pattern input =
  let m =
    Regex.Match(input, String.concat "" pattern)

  if m.Success then Some(m) else None

let getAstKind (context: ASTNode list) index =
  let ret = (List.tryItem index context)
  if ret.IsSome then Some ret.Value.Kind else None

let (?=) a b = a = Some b

let rec recognize (recoList: RecognizeTimesNode list) (context: ASTNode list) =
  
  (** 识别出一个模式节点是否匹配一串AST，返回匹配到的位置索引 *)
//  let recognizeOne (recoNode: RecognizeNode) (context: ASTNode list) =
//  //  console.log ("reco one", recoNode, List.toArray context)
//    match recoNode with
//    | TimesNode m ->
//        let findNotMatch r = m.Kind recognize context r |> not
//          //            console.log("find not match", find (index + r), m.Kind, index, r)
//  //        not ((find r) ?= m.Kind)
//
//        if m.Times > 0 then
//          let notMatch =
//            List.tryFindIndex findNotMatch [ 0 .. m.Times - 1 ]
//
//          if notMatch.IsNone then Some m.Times else None
//        else
//          None
//    | RangeNode m ->
//        let find = getAstKind context
//        let findNotMatch r = not (find r ?= m.Kind)
//
//        let notMatchMin =
//          List.tryFindIndex findNotMatch [ 0 .. m.Min - 1 ]
//        if notMatchMin.IsSome then
//          None
//        else
//          let notMatchMax =
//            List.tryFindIndex findNotMatch [ m.Min .. m.Max - 1 ]
//          //        console.log("range node max", m, notMatchMax)
//
//          if notMatchMax.IsSome then notMatchMax else Some m.Max
//    | RepeatNode m ->
//        let kind = m.Kind()
//  //      console.log("repeat", context |> List.toArray)
//        let mutable ret: int = recognize kind context
//        let mutable step = ret
//
//        while ret < context.Length && step <> 0 do
//          step <- recognize kind context.[ret..]
//          ret <- ret + step
//  //        console.log("repeat", ret, step)
//        Some ret
//     | _ -> None
//  
  let mutable cursor = 0
  let matchEach i =
    if (cursor > context.Length) then false
    else
      let ret = recoList.[i].Kind recognize context.[cursor..]

      if ret.IsSome then
        cursor <- ret.Value + cursor
      ret.IsNone

  let notMatch =
    List.tryFindIndex matchEach [ 0 .. recoList.Length - 1 ]
//  if notMatch.IsSome then
//    console.log ("not match", notMatch, List.toArray recoList, List.toArray context, cursor)
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

let rec eval (context: ASTNode list) (target: string) =
  let evalx kind (progma: Match) =
    let catched = { Kind = kind; Value = progma.Value }
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

let isNextLine (ast: ASTNode) =
  ast.Kind = Indentation NextLine

let guessBehavior (ctx: ASTNode list) =
  let guess (KeyValue(name: BehaviorType, behavior)) =
//    console.log("guessing", name, behavior |> List.toArray, List.toArray ctx)
    if ctx.Length = 0 then
      false
    else
      let fin = recognize behavior ctx
//      console.log("guessing fin", fin)
      fin = ctx.Length
  List.tryFind guess (behaviors |> List.ofSeq)

type Context =
  { kind: ASTKind
    scope: Scope
    elements: ASTNode list }
and Scope = IDictionary<string, Context>

let isValueType kind =
  match kind with
  | ValueType _ -> true
  | _ -> false

let rec parseContext ctx :Context =
  match ctx.kind with
  | ValueType Object ->
    let subContexts = ResizeArray<Context>()
    let mutable cursor = 0
    for i in 1..ctx.elements.Length do
      let unit = ctx.elements.[cursor..i-1]
      match unit with
      | [ head ] ->
        match head.Kind with
        | RightBigParentheses ->
          subContexts.Add { kind = ValueType Object; scope = dict[]; elements = List.empty }
          cursor <- cursor + 1
        | Operator Comma ->
          cursor <- cursor + 1
        | _ ->
          ignore 0
      | [ value; colon; key ] when colon.Kind = Colon && key.Kind = Word && isValueType value.Kind ->
        let subCtx = parseContext { kind = value.Kind; scope = dict[]; elements = [value] }
        let current = subContexts.[subContexts.Count - 1]
        current.scope.Add(key.Value, subCtx)
        cursor <- cursor + 3
      | [ left; sep; key;] when key.Kind = Word && left.Kind = LeftBigParentheses ->
        match sep.Kind with
        | Colon ->
          let sub = subContexts.[subContexts.Count - 1]
          subContexts.RemoveAt(subContexts.Count - 1)
          let current = subContexts.[subContexts.Count - 1]
          current.scope.Add(key.Value, sub)
        | Operator Assignment ->
          ctx.scope.Add(key.Value, subContexts.[0])
          subContexts.Clear()
        | _ ->
          ignore 0
        cursor <- cursor + 3
      | _ ->
//        console.log("expression", i, unit |> List.toArray, recognize behaviors.[BehaviorType.Entry] unit);
        ignore 0
    ctx        
  | _ ->
    ctx        

let mutable stack: ASTNode list = []  

for i, line in seq { for i in 0 .. 14 -> (i, testsLines.[i]) } do
  let ctx = eval stack line
  let expression = ctx @ stack
//  console.log("show ctx", i, expression |> List.toArray)
  match guessBehavior expression with
  | Some behavior ->
    stack <- []
    match behavior with
    | KeyValue(k, _) when k = BehaviorType.AssignInlineObject ->
      let final: Context = { kind = ValueType Object; scope = dict[]; elements = expression }
      console.log(i, "assign inline object", parseContext final)
    | KeyValue(k, _) when k = BehaviorType.AssignObject ->
      console.log(i, "assign object", expression |> List.toArray)
    | _ ->
      console.log(i, behavior.Key, expression |> List.toArray)
  | None ->
    if ctx.Length <> 0 then
      stack <- { Kind = Indentation NextLine; Value = "next line" }::expression

//  if (behavior.IsNone && ctx.Length <> 0) then
//    stack <- AST(Indentation NextLine, "next line")::expression
//  elif behavior.IsSome then
//    stack <- []
//    match behavior with
//    | Some when kind = isAssign ->             
//      console.log({| Key = expression.[2].Value; Value = expression.[0].Value|})

type ASTCollector = { Source: string; Ast: ASTNode [] }
