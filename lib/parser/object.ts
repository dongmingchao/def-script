import { concatRegex, compose } from "../lodash.ts";
import { baseRegexs, useMetaRegex, feedback } from "./index.ts";

const is_nest_object = concatRegex([
  '^',
  baseRegexs.word,
  '|',
  baseRegexs.value_type.number,
  ':$',
]);

function parseNestEntries(source: string) {
  const starter = compose(
		useMetaRegex(/^:/, 'object seperator'),
		useMetaRegex(concatRegex(['^', baseRegexs.word]), 'object key word'),
		useMetaRegex(concatRegex(['^', baseRegexs.value_type.number]), 'object key number'),
  );
  return feedback(source, starter, 'parse nest entries');
}

export { is_nest_object, parseNestEntries };
