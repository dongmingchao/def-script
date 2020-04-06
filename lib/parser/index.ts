import { compose, concatRegex, ComposeContext } from "../lodash.ts";

const baseRegexs = {
	word: /[A-z]\w*/,
	left_parentheses: /\(/,
	right_parentheses: /\)/,
	arrow: / =>/,
	indentation: /\t/,
	operator: {
		assignment: / = ?/,
	},
	value_type: {
		number: /\d+/,
		string: /'(.+)'/,
		boolean: /(true|false)/,
		object: /a/,
	}
}

const object = concatRegex([
	'\\{ (',
  baseRegexs.word,
  '|',
  baseRegexs.value_type.number,
	'): (',
	baseRegexs.value_type.boolean,
	'|',
	baseRegexs.value_type.number,
	'|',
	baseRegexs.value_type.string,
	') \\}'
]);

baseRegexs.value_type.object = object;

export { baseRegexs };

export function useMetaRegex(regexp: RegExp, kind: string, index = 0, cb?: (this: ComposeContext<Core>, c: Core) => void) {
  return function (this: ComposeContext<Core>, c: Core) {
		// console.log(c.source, regexp);
    const match = regexp.exec(c.source)
    if (match === null) return c;
    else {
      c.ast.push({ kind, name: match[index] });
			c.source = c.source.slice(match[0].length);
			cb && cb.call(this, c);
      return c;
    }
  }
}

export interface Core {
  source: string
  ast: AST[]
}

export interface AST {
  kind: string
  name: string
}

export interface Context {
  t: string
  scope: Context[]
  elements: AST[]
}

export function parseIndent(onelineSource: string) {
	let rest = onelineSource;
	let indent = 0;
	if (onelineSource.startsWith('\t')) {
		const par = onelineSource.match(/\t/g);
		if (par === null) throw SyntaxError('this error never show about indent');
		indent = par.length;
		rest = rest.slice(indent);
	}
	return { indent, rest };
}

export function feedback(source: string, starter: (c: Core) => Core, errMsg: string) {
	const { ast, source: rest } = starter({ source, ast: []});
	const pos = source.length - rest.length + 1;
	const mark = '^'.padStart(pos);
  if (rest.length) throw new SyntaxError(`${errMsg} error: '${rest}'\n${source}\n${mark}`);
  return ast;
}
