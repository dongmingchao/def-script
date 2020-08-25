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
       Arrow = " =>"
       Operator = {| Assignment = " = ?" |}
       ValueType = {| Number = "\d+"
                      String = "'(.+)'"
                      Boolean = "(true|false)" |} |}

type Operator = | Assignment
type ValueType = | Number | String | Boolean
type ASTKind =
    | Word | LeftParentheses | RightParentheses | Arrow | Indentation
    | S | ValueType
    
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
