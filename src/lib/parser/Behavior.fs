module lib.parser.Behavior

open lib.core.BuildInRegex

type RepeatType =
    | Black
    | White

type RecognizeTimesNode = { Kind: ASTKind; Times: int }

type RecognizeRangeNode = { Kind: ASTKind; Max: int; Min: int }

type RecognizeRepeatNode =
    { Type: RepeatType
      Kind: unit -> RecognizeNode list }

and RecognizeNode =
    | TimesNode of RecognizeTimesNode
    | RangeNode of RecognizeRangeNode
    | RepeatNode of RecognizeRepeatNode

type Behavior =
    { Kind: RecognizeNode list
      Name: string }

let isFunctionDeclare: RecognizeNode list =
    [ TimesNode { Kind = Operator Arrow; Times = 1 }
      TimesNode { Kind = RightParentheses; Times = 1 }
      RangeNode { Kind = Word; Max = 1; Min = 0 }
      RepeatNode
          { Type = White
            Kind =
                fun () ->
                    [ TimesNode { Kind = Operator Comma; Times = 1 }
                      TimesNode { Kind = Word; Times = 1 } ] }
      TimesNode { Kind = LeftParentheses; Times = 1 }
      TimesNode { Kind = Word; Times = 1 } ]

let isAssign: RecognizeNode list =
    [ TimesNode
        { Kind = Operator Assignment
          Times = 1 }
      TimesNode { Kind = Word; Times = 1 } ]

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

let rec isEntry (): RecognizeNode list =
    [ RangeNode
        { Kind = ValueType String
          Max = 1
          Min = 0 }
      RangeNode
          { Kind = ValueType Number
            Max = 1
            Min = 0 }
      RangeNode
          { Kind = ValueType Boolean
            Max = 1
            Min = 0 }
      TimesNode { Kind = Colon; Times = 1 }
      TimesNode { Kind = Word; Times = 1 }
      RepeatNode
          { Type = White
            Kind = fun () -> [ TimesNode { Kind = Indentation Tab; Times = 1 } ] }
      TimesNode
          { Kind = Indentation NextLine
            Times = 1 }]

let rec isInlineObject (): RecognizeNode list =
    [ TimesNode
        { Kind = RightBigParentheses
          Times = 1 }
      RepeatNode
          { Type = White
            Kind =
                fun () ->
                    [ RangeNode
                        { Kind = ValueType String
                          Max = 1
                          Min = 0 }
                      RangeNode
                          { Kind = ValueType Number
                            Max = 1
                            Min = 0 }
                      RangeNode
                          { Kind = ValueType Boolean
                            Max = 1
                            Min = 0 }
                      RepeatNode { Kind = isInlineObject; Type = White }
                      TimesNode { Kind = Colon; Times = 1 }
                      TimesNode { Kind = Word; Times = 1 }
                      TimesNode { Kind = Operator Comma; Times = 1 } ] }
      TimesNode { Kind = LeftBigParentheses; Times = 1 } ]

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
let isAssignInlineObject () = isInlineObject () @ isAssign
//let isAssignObject () = isEntry () @ isAssign

let behaviorSet =
    [ { Kind = TimesNode { Kind = ValueType String; Times = 1 }::isAssign
        Name = "assign string" }
      { Kind = TimesNode { Kind = ValueType Number; Times = 1 }::isAssign
        Name = "assign number" }
      { Kind = TimesNode { Kind = ValueType Boolean; Times = 1 }::isAssign
        Name = "assign bool" }
      { Kind = isAssignInlineObject ()
        Name = "assign inline object" }
      { Kind = isAssign
        Name = "assign" }
      { Kind = isEntry ()
        Name = "entry" }
      { Kind = isFunctionDeclare
        Name = "function declare" } ]
