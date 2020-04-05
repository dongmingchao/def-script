import { Context, AST } from './../parser/index.ts';

export function stringify(c: Context, deep = 0) {
  let base = '';
  switch(c.t) {
    case 'global':
      base += c.scope.map(e => stringify(e)).join('\n');
      break;
    case 'assignment':
      const sep = new Array(deep).fill('\t');
      sep.unshift('\n');
      base += stringifyAssignment(c).join(sep.join(''));
      break;
    case 'function':
      base += stringifyFunction(c, deep);
      break;
  }
  return base;
}

function stringifyAssignment({elements}: Context) {
  const [{name: left}, {name: op}, vars, ...rest]: AST[] = elements;
  if (['number', 'boolean', 'string'].includes(vars.kind)) {
    return [`let ${left}${op}${vars.name};`];
  }
  if (vars.kind === 'object start') {
    const ret: string[] = [];
    ret.push(`let ${left}${op}${vars.name}`);
    let base = '';
    rest.forEach(e => {
      switch(e.kind) {
        case 'comma':
          base += e.name;
          ret.push(base);
          base = '';
          break;
        case 'object key word':
        case 'object key number':
          base += '\t' + e.name;
          break;
        case 'number':
        case 'boolean':
        case 'string':
        case 'object seperator':
          base += e.name;
          break;
        case 'object end':
          ret.push(base, '}');
          break;
      }
    });
    return ret;
  }
  return [];
}

function stringifyFunction({elements, scope}: Context, deep: number) {
  const [{name}, ,...args] = elements;
  args.pop();
  const indent = new Array(deep + 1).fill('\t').join('');
  const eofIndent = new Array(deep).fill('\t').join('');
  const in_scope = scope.map(e => stringify(e, deep + 1)).join('\n' + indent);
  return `function ${name}(${args.map(e => e.name).join('')}) {\n` +
          `${indent}${in_scope}\n${eofIndent}}`;
}
