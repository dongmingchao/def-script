﻿module lib.core.BuildInRegex

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
    | Arrow | Comma

type KeyWord =
    | If
    | For

and ValueType =
    | Number
    | String
    | Boolean

and ASTKind =
    | Word
    | LeftParentheses
    | RightParentheses
    | Indentation
    | Operator of Operator
    | KeyWord of KeyWord
    | ValueType of ValueType
//let baseRegexs = {
//	word: "[A-z]\w*",
//	left_parentheses: "\(",
//	right_parentheses: "\)",
//	arrow: " =>",
//	indentation: "\t",
//	operator: {
//		assignment: " = ?",
//	},
//	value_type: {
//		number: "\d+",
//		string: "'(.+)'",
//		boolean: "(true|false)",
//		object: "a",
//	}
//}
