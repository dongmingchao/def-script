module lib.parser.Behavior

open lib.core.BuildInRegex

type RepeatType =
    | Black
    | White

type RecognizeTimesNode = { Kind: ASTKind option -> bool; Times: int }

type RecognizeRangeNode = { Kind: ASTKind; Max: int; Min: int }

type RecognizeOneOfNode = { Kinds: ASTKind list }

type RecognizeRepeatNode =
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

let isASTKind t p =
    match p with
    | Some m ->
        m = t
    | _ -> false

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

let isAssign: RecognizeNode list =
    [ TimesNode
        { Kind = Operator Assignment |> isASTKind
          Times = 1 }
      TimesNode { Kind = isASTKind Word; Times = 1 } ]

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

let IsValueType (kind: ASTKind option) =
  match kind with
  | Some m ->
    match m with
    | ValueType _ ->
        true
    | _ -> false
  | _ ->
    false

let rec isEntry (): RecognizeNode list =
    [ TimesNode { Kind = IsValueType; Times = 1 }
      RepeatNode { Kind = isEntry; Type = White }
      TimesNode { Kind = isASTKind Colon; Times = 1 }
      TimesNode { Kind = isASTKind Word; Times = 1 }
      RepeatNode
          { Type = White
            Kind = isIndent }
      TimesNode
          { Kind = Indentation NextLine |> isASTKind
            Times = 1 }]

let rec isInlineEntry () = [
                     TimesNode { Kind = IsValueType; Times = 1 }
                     RepeatNode { Kind = isInlineObject; Type = White }
                     TimesNode { Kind = isASTKind Colon; Times = 1 }
                     TimesNode { Kind = isASTKind Word; Times = 1 }
                     TimesNode { Kind = Operator Comma |> isASTKind; Times = 1 } ]
and isInlineObject (): RecognizeNode list =
    [ TimesNode
        { Kind = isASTKind RightBigParentheses
          Times = 1 }
      RepeatNode
          { Type = White
            Kind = isInlineEntry }
      TimesNode { Kind = isASTKind LeftBigParentheses; Times = 1 } ]

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
let isAssignInlineObject () = isInlineObject() @ isAssign
//let isAssignObject () = isEntry () @ isAssign

type BehaviorType =
    | AssignString
    | AssignNumber
    | AssignBoolean
    | AssignInlineObject
    | AssignObject
    | FunctionDeclare

let behaviors = dict [
    AssignString, TimesNode { Kind = ValueType String |> isASTKind; Times = 1 }::isAssign
    AssignNumber, TimesNode { Kind = ValueType Number |> isASTKind; Times = 1 }::isAssign
    AssignBoolean, TimesNode { Kind = ValueType Boolean |> isASTKind; Times = 1 }::isAssign
    AssignInlineObject, isAssignInlineObject()
    AssignObject, isEntry() @ isAssign
    FunctionDeclare, isFunctionDeclare
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
