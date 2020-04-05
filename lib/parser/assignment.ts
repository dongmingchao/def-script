import { concatRegex, compose } from "../lodash.ts";
import { baseRegexs, useMetaRegex, feedback } from "./index.ts";

const is_assignment = concatRegex([
	'^',
	baseRegexs.word,
	baseRegexs.operator.assignment,
  baseRegexs.value_type.object,
	'|',
	baseRegexs.value_type.boolean,
	'|',
	baseRegexs.value_type.number,
	'|',
	baseRegexs.value_type.string,
	'|',
	baseRegexs.word,
	'$'
]);

function parseAssignment(source: string) {
	const parseValueType = compose(
		useMetaRegex(concatRegex(['^', baseRegexs.value_type.boolean]), 'boolean'),
		useMetaRegex(concatRegex(['^', baseRegexs.value_type.string]), 'string'),
		useMetaRegex(concatRegex(['^', baseRegexs.value_type.number]), 'number'),
	);
	const parseObject = compose(
		parseValueType,
		useMetaRegex(/^: /, 'object seperator'),
		useMetaRegex(concatRegex(['^', baseRegexs.word]), 'object key word'),
		useMetaRegex(concatRegex(['^', baseRegexs.value_type.number]), 'object key number'),
	)
	const repeatComma = useMetaRegex(/^, /, 'comma', 0, function(c) {
		this.arr.splice(1, 0, parseObject);
		this.arr.splice(1, 0, repeatComma);
	});
  const parseValue = compose(
		useMetaRegex(/^ \}$/, 'object end'),
		repeatComma,
		parseObject,
		useMetaRegex(/^\{ /, 'object start'),
		useMetaRegex(/^[A-z]\w*$/, 'word'),
		parseValueType,
  );
  const starter = compose(
    parseValue,
		useMetaRegex(/^ = /, 'operator'),
		useMetaRegex(/^[A-z]\w*/, 'word'),
  );
  return feedback(source, starter, 'parse assignment');
}

export { is_assignment, parseAssignment }
