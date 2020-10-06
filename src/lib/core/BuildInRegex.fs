module lib.core.BuildInRegex

//[<Literal>]
//let Word = "[A-z]\w*"
//[<Literal>]
//let LeftParentheses = "\("
//let RightParentheses = "\)"
//let Arrow = " =>"
//let Indentation = "\t"
//let Assignment = " = ?"
//type Operator = | Assignment
//
//[<Literal>]
//let Number = "\d+"
//[<Literal>]
//let String = "'(.+)'"
//[<Literal>]
//let Boolean = "(true|false)"

let BuildInRegexs =
  {| Word = "[A-z]\w*"
     LeftParentheses = "\("
     RightParentheses = "\)"
     LeftBigParentheses = "\{ "
     RightBigParentheses = " \}"
     Colon = ": "
     KeyWord = {| If = "if"; For = "for" |}
     Operator =
       {| Assignment = " = ?"
          Arrow = " =>"
          Comma = ", " |}
     ValueType =
       {| Number = "\d+"
          String = "'(.+)'"
          Boolean = "(true|false)" |} |}

type Operator =
  | Assignment
  | Arrow
  | Comma

type Indentation =
  | NextLine
  | Tab

type KeyWord =
  | If
  | For

and ValueType =
  | Number
  | String
  | Boolean
  | Object

and ASTKind =
  | Word
  | LeftParentheses
  | RightParentheses
  | LeftBigParentheses
  | RightBigParentheses
  | Colon
  | Indentation of Indentation
  | Operator of Operator
  | KeyWord of KeyWord
  | ValueType of ValueType


type ASTConfig =
  { Matcher: string
    Kind: ASTKind
    Name: string }

let rest =
  [ { Matcher = BuildInRegexs.ValueType.Boolean
      Kind = ValueType Boolean
      Name = "boolean" }
    { Matcher = BuildInRegexs.ValueType.Number
      Kind = ValueType Number
      Name = "number" }
    { Matcher = BuildInRegexs.ValueType.String
      Kind = ValueType String
      Name = "string" }
    { Matcher = BuildInRegexs.Colon
      Kind = Colon
      Name = "Colon" }
    { Matcher = BuildInRegexs.LeftBigParentheses
      Kind = LeftBigParentheses
      Name = "LeftBigParentheses" }
    { Matcher = BuildInRegexs.RightBigParentheses
      Kind = RightBigParentheses
      Name = "RightBigParentheses" }
    { Matcher = BuildInRegexs.Word
      Kind = Word
      Name = "word" }
    { Matcher = BuildInRegexs.Operator.Assignment
      Kind = Operator Assignment
      Name = "word" }
    { Matcher = "\t"
      Kind = Indentation Tab
      Name = "tab" } ]
