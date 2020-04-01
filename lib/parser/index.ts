import { compose } from "../lodash.ts";

export function useMetaRegex(regexp: RegExp, kind: string, index = 0) {
  return function (c: Core) {
    const match = regexp.exec(c.source)
    if (match === null) return c;
    else {
      c.ast.push({ kind, name: match[index] });
      c.source = c.source.slice(match[0].length);
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

export function parseFunction(source: string) {
  const starter = compose(
		useMetaRegex(/^\) =>$/, 'declare func end'),
		useMetaRegex(/^((, )?([A-z]\w*))+/, 'arguments'),
		useMetaRegex(/^\(/, 'declare func start'),
		useMetaRegex(/^[A-z]\w+/, 'word'),
  );
  return feedback(source, starter, 'parse function');
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

export function parseAssignment(source: string) {
  const parseValue = compose(
    useMetaRegex(/^'(.+)'$/, 'string', 1),
    useMetaRegex(/^\d+$/, 'number'),
    useMetaRegex(/^true|false$/, 'boolean'),
		useMetaRegex(/^[A-z]\w*$/, 'word'),
  );
  const starter = compose(
    parseValue,
		useMetaRegex(/^ = /, 'operator'),
		useMetaRegex(/^[A-z]\w*/, 'word'),
  );
  return feedback(source, starter, 'parse assignment');
}

function feedback(source: string, starter: (c: Core) => Core, errMsg: string) {
  const { ast, source: rest } = starter({ source, ast: []});
  if (rest.length) throw new SyntaxError(`${errMsg} error: ${rest}`);
  return ast;
}
