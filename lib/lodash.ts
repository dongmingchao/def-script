export function compact(input: string[] | []) {
  return input.filter(e => e !== '' && e != null);
}

export type ComposeContext<A> = {
  curi: number,
  arr: Array<(this: ComposeContext<A>, a: A) => A>,
};

export function compose<A>( ...f: ComposeContext<A>['arr'] ): (arg: A) => A {
  return function (arg: A){
    let count = 0, ret:(A | undefined) = undefined;
    while(f.length > count) {
      ret = f.reduceRight<A>((total: A, cur: (a: A) => A, curi, arr) => {
        count++;
        return cur.call({ arr, curi }, total);
      }, arg);
    }
    return ret as A;
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
