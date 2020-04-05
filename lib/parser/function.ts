import { concatRegex } from "../lodash.ts";
import { baseRegexs } from "./index.ts";

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

export { is_function }
