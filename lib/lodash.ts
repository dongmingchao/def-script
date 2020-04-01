export function compact(input: string[] | []) {
  return input.filter(e => e !== '' && e != null);
}

export function compose<A>( ...f: Array<(a: A) => A> ) {
  return function (arg: A){
    return f.reduceRight((total: A, cur: (a: A) => A) => {
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
