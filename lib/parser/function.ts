import { concatRegex, compose } from "../lodash.ts";
import { baseRegexs, useMetaRegex, feedback } from "./index.ts";

const is_function = concatRegex([
	'^',
	baseRegexs.word,
	baseRegexs.left_parentheses,
	'((, )?(', 
	baseRegexs.word,
	'))*',
	baseRegexs.right_parentheses,
	baseRegexs.arrow,
	'$'
]);

export function parseFunction(source: string) {
	const parseWord = useMetaRegex(concatRegex(['^', baseRegexs.word]), 'word');
	const repeatComma = useMetaRegex(/^, /, 'comma', 0);

  const starter = compose(
		useMetaRegex(/^\) =>$/, 'declare func end'),
		repeatComma,
		parseWord,
		useMetaRegex(/^\(/, 'declare func start'),
		parseWord,
  );
  return feedback(source, starter, 'parse function');
}

export { is_function }
