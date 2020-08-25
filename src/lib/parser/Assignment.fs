module lib.parser.Assignment

open lib.core
open lib.core.BuildInRegex

let isAssignment: string list = [
  BuildInRegexs.Word
  BuildInRegexs.Operator.Assignment
  "("
  //  	BuildInRegexs.ValueType.object,
//  "|"
  BuildInRegexs.ValueType.Boolean
  "|"
  BuildInRegexs.ValueType.Number
  "|"
  BuildInRegexs.ValueType.String
  "|"
  BuildInRegexs.Word
  ")"
]