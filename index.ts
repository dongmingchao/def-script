#!/usr/bin/env -S deno --allow-read --allow-write
// import {
//   writeFileStr,
//   readFileStr,
// } from "https://deno.land/std/fs/mod.ts";
import { compact, compose, concatRegex } from "./lib/lodash.ts";
import { useMetaRegex, parseIndent, parseFunction, AST, Context } from "./lib/parser/index.ts";
import { stringify } from "./lib/stringifier/index.ts";
import { is_assignment, parseAssignment } from "./lib/parser/assignment.ts";
import { is_function } from "./lib/parser/function.ts";

const base = `
var_obj = { key1: 'val1' }
var_obj = { key1: 'val1', key2: 1 }
var_obj = { key1: 'val1', key2: 1, key3: true }
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
			t: 'assignment',
			scope: [],
			elements: ast,
		})
	}
	return ctxs;
}

console.log(is_assignment);

parseExpression(base);
