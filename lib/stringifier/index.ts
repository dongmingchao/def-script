import { Context, AST } from './../parser/index.ts';

export function stringify(c: Context, deep = 0) {
  let base = '';
  switch(c.t) {
    case 'global':
      base += c.scope.map(e => stringify(e)).join('\n');
      break;
    case 'variable':
      base += stringifyVariable(c);
      break;
    case 'function':
      base += stringifyFunction(c, deep);
      break;
  }
  return base;
}

function stringifyVariable({elements}: Context) {
  const [{name: left}, {name: op}, {name: right}]: AST[] = elements;
  return `let ${left}${op}${right};`;
}

function stringifyFunction({elements, scope}: Context, deep: number) {
  const [{name}, ,{name: args}] = elements;
  const indent = new Array(deep + 1).fill('\t').join('');
  const eofIndent = new Array(deep).fill('\t').join('');
  const in_scope = scope.map(e => stringify(e, deep + 1)).join('\n' + indent);
  return `function ${name}(${args}) {\n${indent}${in_scope}\n${eofIndent}}`
}
