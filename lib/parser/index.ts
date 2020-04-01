import { compose } from "../lodash.ts";

export function useMetaRegex(regexp: RegExp, kind: string, index = 0) {
  return function (str: string) {
    const match = regexp.exec(str)
    if (match === null) return str;
    else {
      console.log({ kind, name: match[index] });
      return str.slice(match[0].length);
    }
  }
}

export function parseFunction(onelineSource: string) {
  const starter = compose(
		useMetaRegex(/^\) =>$/, 'declare func end'),
		useMetaRegex(/^((, )?([A-z]\w*))+/, 'arguments'),
		useMetaRegex(/^\(/, 'declare func start'),
		useMetaRegex(/^[A-z]\w+/, 'word'),
  );
  const rest = starter(onelineSource);
  if (rest.length) throw new SyntaxError('parse function error');
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
    useMetaRegex(/^'([^']+)'$/, 'string', 1),
    useMetaRegex(/^\d+$/, 'number'),
    useMetaRegex(/^true|false$/, 'boolean'),
		useMetaRegex(/^[A-z]\w*$/, 'word'),
  );
  const starter = compose(
    parseValue,
		useMetaRegex(/^ = /, 'operator'),
		useMetaRegex(/^[A-z]\w*/, 'word'),
  );
  const rest = starter(source);
  if (rest.length) throw new SyntaxError('parse assignment error: ' + rest);
}
