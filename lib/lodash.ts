export function compact(input: string[] | []) {
  return input.filter(e => e !== '' && e != null);
}

export function compose( ...f: Array<(a: string) => string> ) {
  return function (arg: string){
    return f.reduceRight((total: string, cur: (a: string) => string) => {
      return cur(total);
    }, arg);
  }
}

export function concatRegex(regex: (RegExp|string)[], flags = '') {
  const sources = regex.reduce((prev, curr) => {
    if (curr instanceof RegExp) {
      return prev + curr.source;
    }
     return prev + curr;
  }, '');
  return new RegExp(sources, flags);
}
