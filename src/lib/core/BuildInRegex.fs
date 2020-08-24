module lib.core.BuildInRegex

[<Literal>]
let Word = "[A-z]\w*"
[<Literal>]
let LeftParentheses = "\("
let RightParentheses = "\)"
let Arrow = " =>"
let Indentation = "\t"
let Assignment = " = ?"
type Operator = | Assignment

[<Literal>]
let Number = "\d+"
[<Literal>]
let String = "'(.+)'"
[<Literal>]
let Boolean = "(true|false)"

type Customer = 
    { First: "sadsa"
      Last: string;
      SSN: uint32
      AccountNumber: uint32; }

type ValueType = | Number | String | Boolean
type BuildInRegexs =
    | Word | LeftParentheses | RightParentheses| Arrow | Indentation
    | Operator | ValueType

let getBuildInRegex = function
    | ValueType.String -> "123"

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
