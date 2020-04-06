#!/usr/bin/env -S deno --allow-read --allow-write
// import {
//   writeFileStr,
//   readFileStr,
// } from "https://deno.land/std/fs/mod.ts";
import { compact, compose, concatRegex } from "./lib/lodash.ts";
import { useMetaRegex, parseIndent, AST, Context, baseRegexs } from "./lib/parser/index.ts";
import { stringify } from "./lib/stringifier/index.ts";
import { is_assignment, parseAssignment } from "./lib/parser/assignment.ts";
import { is_function, parseFunction } from "./lib/parser/function.ts";
import { is_nest_object, parseNestEntries } from "./lib/parser/object.ts";

const base = `
var_obj6 =
	key2: 'val2'
	number_key: 1
	bool_key: true
	nest_key:
		key3: 'val3'
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
	// 去除缩进，转化为作用域索引
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
		console.log('assignment', rest);
		const nContext = {
			t: 'assignment',
			scope: [],
			elements: ast,
		};
		if (ast.length < 3) {
			ctxs.push(nContext);
		}
		currContext.scope.push(nContext)
	} else if (is_nest_object.test(rest)) {
		const ast = parseNestEntries(rest);
		const nContext = {
			t: 'object',
			scope: [],
			elements: ast,
		};
		ctxs.push(nContext);
		currContext.scope.push(nContext);
	} else {
		console.error('Uncaught Source:', rest);
	}
	return ctxs;
}

console.log(is_assignment);
parseExpression(base);
