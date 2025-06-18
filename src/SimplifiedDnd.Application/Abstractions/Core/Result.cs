using System.Diagnostics.CodeAnalysis;

namespace SimplifiedDnd.Application.Abstractions.Core;

public class Result {
  [MemberNotNullWhen(false, nameof(Error))]
  public bool IsSuccess => Error is null;
  
  public DomainError? Error { get; private init; }

  protected Result(DomainError? error) => Error = error;

  public static implicit operator Result(DomainError error) => new(error);

  public static Result Failure(DomainError error) => new(error);
  
  public static Result<TValue> TypedFailure<TValue>(DomainError error) where TValue : notnull =>
    new(default, error);
}

public class Result<TValue>(
  TValue? value,
  DomainError? error
) : Result(error)
  where TValue : notnull {
  public TValue Value => value ??
    throw new InvalidOperationException("The value of a failure result can not be accessed.");

  public static implicit operator Result<TValue>(TValue value) => new(value, null);
  public static implicit operator Result<TValue>(DomainError error) => new(default, error);
}