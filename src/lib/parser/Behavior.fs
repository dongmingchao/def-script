module lib.parser.Behavior

open lib.core.BuildInRegex
open Fable.Core.JS

type RepeatType =
    | Black
    | White

type ASTNode = {
    Kind: ASTKind
    Value: string
}


type RecognizeRangeNode = { Kind: ASTKind; Max: int; Min: int }

type RecognizeOneOfNode = { Kinds: ASTKind list }

type RecognizeTimesNode = { Kind: (RecognizeTimesNode list -> ASTNode list -> int) -> ASTNode list -> int option; Times: int }
and RecognizeRepeatNode =
    { Type: RepeatType
      Kind: unit -> RecognizeNode list }

and RecognizeNode =
    | TimesNode of RecognizeTimesNode
    | RangeNode of RecognizeRangeNode
    | OneOfNode of RecognizeOneOfNode
    | RepeatNode of RecognizeRepeatNode

type Behavior =
    { Kind: RecognizeNode list
      Name: string }

let isRepeatNode part (recognize: RecognizeTimesNode list -> ASTNode list -> int) (context: ASTNode list) =
    let mutable ret = 0
    let mutable step = -1

    while ret < context.Length && step <> 0 do
      step <- recognize part context.[ret..]
      ret <- ret + step
    Some ret

let isASTKind (t: ASTKind) (recognize: RecognizeTimesNode list -> ASTNode list -> int) (p: ASTNode list) =
    let m = List.tryHead p
    if m.IsSome then
        if m.Value.Kind = t then Some 1
        else None
    else None

let isArgumentsNode () =
    [ TimesNode { Kind = Operator Comma |> isASTKind; Times = 1 }
      TimesNode { Kind = isASTKind Word; Times = 1 } ]

let isFunctionDeclare: RecognizeNode list =
    [ TimesNode { Kind = Operator Arrow |> isASTKind; Times = 1 }
      TimesNode { Kind = isASTKind RightParentheses; Times = 1 }
      RangeNode { Kind = Word; Max = 1; Min = 0 }
      RepeatNode
          { Type = White
            Kind = isArgumentsNode }
      TimesNode { Kind = isASTKind LeftParentheses; Times = 1 }
      TimesNode { Kind = isASTKind Word; Times = 1 } ]

let isAssign =
    [{ Kind = Operator Assignment |> isASTKind
       Times = 1 }
     { Kind = isASTKind Word; Times = 1 }]

//let isAssignString: RecognizeNode list =
//    TimesNode { Kind = ValueType String; Times = 1 }
//    :: isAssign
//
//let isAssignNumber: RecognizeNode list =
//    TimesNode { Kind = ValueType Number; Times = 1 }
//    :: isAssign
//
//let isAssignBool: RecognizeNode list =
//    TimesNode { Kind = ValueType Boolean; Times = 1 }
//    :: isAssign

let isIndent () = [TimesNode { Kind = Indentation Tab |> isASTKind; Times = 1 }]

let IsValueTypeAnd (others: unit -> RecognizeTimesNode list) (recognize: RecognizeTimesNode list -> ASTNode list -> int) (context: ASTNode list) =
  match context.Head.Kind with
  | ValueType _ ->
      Some 1
  | _ ->
      let matched = recognize (others()) context
      if (matched = 0) then None
      else Some matched

//let rec isEntry (): RecognizeNode list =
//    [ TimesNode { Kind = IsValueTypeAnd isEntry; Times = 1 }
//      TimesNode { Kind = isASTKind Colon; Times = 1 }
//      TimesNode { Kind = isASTKind Word; Times = 1 }
//      RepeatNode
//          { Type = White
//            Kind = isIndent }
//      TimesNode
//          { Kind = Indentation NextLine |> isASTKind
//            Times = 1 }]
//
let rec isInlineEntry () = [
   { Kind = IsValueTypeAnd isInlineObject; Times = 1 }
   { Kind = isASTKind Colon; Times = 1 }
   { Kind = isASTKind Word; Times = 1 }
   { Kind = Operator Comma |> isASTKind; Times = 1 } ]
//and isInlineObject (): RecognizeNode list =
//    [ TimesNode
//        { Kind = isASTKind RightBigParentheses
//          Times = 1 }
//      RepeatNode
//          { Type = White
//            Kind = isInlineEntry }
//      TimesNode { Kind = isASTKind LeftBigParentheses; Times = 1 } ]

//and isObject: RecognizeNode list =
//  [ TimesNode
//      { Kind = RightBigParentheses
//        Times = 1 }
//    RepeatNode
//      { Type = White
//        Kind =
//          [ RangeNode
//              { Kind = ValueType String
//                Max = 1
//                Min = 0 }
//            RangeNode
//              { Kind = ValueType Number
//                Max = 1
//                Min = 0 }
//            RangeNode
//              { Kind = ValueType Boolean
//                Max = 1
//                Min = 0 }
//            RepeatNode { Kind = isMetaObject; Type = White; }
//            TimesNode { Kind = Colon; Times = 1 }
//            TimesNode { Kind = Word; Times = 1 }
//            TimesNode { Kind = Operator Comma; Times = 1 } ] }
//    TimesNode { Kind = LeftBigParentheses; Times = 1 } ]
//
//let isAssignInlineObject () = isInlineObject() @ isAssign
//let isAssignObject () = isEntry () @ isAssign

and isInlineObject () = [
    { Kind = isASTKind RightBigParentheses; Times = 1 }
    { Kind = isRepeatNode (isInlineEntry()); Times = 1 }
    { Kind = isASTKind LeftBigParentheses; Times = 1 }
]

type BehaviorType =
    | AssignString
    | AssignNumber
    | AssignBoolean
    | AssignInlineObject
    | AssignObject
    | FunctionDeclare

//let behaviors = dict [
//    AssignString, TimesNode { Kind = ValueType String |> isASTKind; Times = 1 }::isAssign
//    AssignNumber, TimesNode { Kind = ValueType ValueType.Number |> isASTKind; Times = 1 }::isAssign
//    AssignBoolean, TimesNode { Kind = ValueType Boolean |> isASTKind; Times = 1 }::isAssign
//    AssignInlineObject, isAssignInlineObject()
//    AssignObject, isEntry() @ isAssign
//    FunctionDeclare, isFunctionDeclare
//]

let behaviors = dict [
    AssignString, { Kind = ValueType String |> isASTKind; Times = 1 }::isAssign
    AssignNumber, { Kind = ValueType ValueType.Number |> isASTKind; Times = 1 }::isAssign
    AssignBoolean, { Kind = ValueType Boolean |> isASTKind; Times = 1 }::isAssign
    AssignInlineObject, isInlineObject() @ isAssign
]

//let behaviorSet =
//    [ { Kind = TimesNode { Kind = ValueType String; Times = 1 }::isAssign
//        Name = "assign string" }
//      { Kind = TimesNode { Kind = ValueType Number; Times = 1 }::isAssign
//        Name = "assign number" }
//      { Kind = TimesNode { Kind = ValueType Boolean; Times = 1 }::isAssign
//        Name = "assign bool" }
//      { Kind = isAssignInlineObject ()
//        Name = "assign inline object" }
//      { Kind = isAssign
//        Name = "assign" }
//      { Kind = isEntry ()
//        Name = "entry" }
//      { Kind = isFunctionDeclare
//        Name = "function declare" } ]
