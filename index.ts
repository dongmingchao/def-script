#!/usr/bin/env -S deno --allow-read --allow-write
// import {
//   writeFileStr,
//   readFileStr,
// } from "https://deno.land/std/fs/mod.ts";
import { compact, compose, concatRegex } from "./lib/lodash.ts";
import { useMetaRegex, parseIndent, parseFunction, parseAssignment } from "./lib/parser/index.ts";

const base = `
func(args1, args2) =>
	local_var_number = 34
	local_var_bool = false
	local_var_str = 'local str()asdsadw'[].{}:Dsa12=>'
	local_equal = args1
`

function parseExpression(program: string) {
  compact(program.split('\n')).map(parseExpressionLine);
}

const mixed = compose(
	useMetaRegex(/^\(/, 'left parentheses'),
	useMetaRegex(/^\)/, 'right parentheses'),
	useMetaRegex(/^'([^']+)'/, 'string', 1),
	useMetaRegex(/^\d+/, 'number'),
	useMetaRegex(/^ = /, 'operator'),
	useMetaRegex(/^[A-z]\w+/, 'word'),
	useMetaRegex(/^\t/, 'indentation'),
);

function parseExpressionLine(onelineSource: string) {
	const { indent, rest } = parseIndent(onelineSource);
	if (is_function.test(rest)) {
		parseFunction(rest);
	} else if(is_assignment.test(rest)) {
		parseAssignment(rest);
	}
	console.log('-----------------------------');
}

const baseRegexs = {
	word: /[A-z]\w*/,
	left_parentheses: /\(/,
	right_parentheses: /\)/,
	arrow: / =>/,
	indentation: /\t/,
	operator: {
		assignment: / = /,
	},
	value_type: {
		number: /\d+/,
		string: /'(.+)'/,
		boolean: /true|false/,
	}
}

const is_function = concatRegex([
	'^',
	baseRegexs.word,
	baseRegexs.left_parentheses,
	'((, )?(', 
	baseRegexs.word,
	'))+',
	baseRegexs.right_parentheses,
	baseRegexs.arrow,
	'$'
]);

const is_assignment = concatRegex([
	'^',
	baseRegexs.word,
	baseRegexs.operator.assignment,
	'(',
	baseRegexs.value_type.boolean,
	'|',
	baseRegexs.value_type.number,
	'|',
	baseRegexs.value_type.string,
	'|',
	baseRegexs.word,
	')$'
])

parseExpression(base);