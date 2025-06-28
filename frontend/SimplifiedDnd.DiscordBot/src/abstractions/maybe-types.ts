export enum MaybeType {
  Just = 'maybe-type__just',
  DomainError = 'maybe-type__error',
}

interface Just<T> {
  type: typeof MaybeType.Just;
  value: T;
}

interface DomainError {
  type: typeof MaybeType.DomainError;
  code: string;
  detail: string;
}

export type Maybe<T> = Just<T> | DomainError;

export const DomainError = (code: string, detail: string): DomainError => ({
  code,
  detail,
  type: MaybeType.DomainError,
});

export const Just = <T>(value: T): Just<T> => ({
  type: MaybeType.Just,
  value,
});
