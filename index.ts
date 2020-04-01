#!/usr/bin/env -S deno --allow-read --allow-write
// import {
//   writeFileStr,
//   readFileStr,
// } from "https://deno.land/std/fs/mod.ts";
import { compact, compose, concatRegex } from "./lib/lodash.ts";
import { useMetaRegex, parseIndent, parseFunction, parseAssignment, AST, Context } from "./lib/parser/index.ts";
import { stringify } from "./lib/stringifier/index.ts";

const base = `
var_str = 'str'
var_number = 12

func(args1, args2) =>
	local_var_number = 34
	local_var_bool = false
	local_var_str = 'local str()asdsadw'[].{}:Dsa12=>'
	func(args3, args4) =>
		local_equal = args1

var_boolean = true
`

function parseExpression(program: string) {
	const startCtx: Context = {
		t: 'global',
		scope: [],
		elements: [],
	}
	const afterComplieContext = compact(program.split('\n')).reduce(parseExpressionLine, [startCtx]);
	console.log(JSON.stringify(afterComplieContext[0], null, 2));
	console.log(stringify(afterComplieContext[0]))
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

function parseExpressionLine(ctxs: Context[], onelineSource: string): Context[] {
	const { indent, rest } = parseIndent(onelineSource);
	const currContext = ctxs[indent];
	if (is_function.test(rest)) {
		const ast = parseFunction(rest);
		const nContext = {
			t: 'function',
			scope: [],
			elements: ast,
		};
		ctxs.push(nContext);
		currContext.scope.push(nContext);
	} else if(is_assignment.test(rest)) {
		const ast = parseAssignment(rest);
		currContext.scope.push({
			t: 'variable',
			scope: [],
			elements: ast,
		})
	}
	return ctxs;
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