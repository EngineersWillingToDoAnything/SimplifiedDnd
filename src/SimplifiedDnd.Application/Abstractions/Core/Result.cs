using System.Diagnostics.CodeAnalysis;

namespace SimplifiedDnd.Application.Abstractions.Core;

public class Result {
  [MemberNotNullWhen(false, nameof(Error))]
  public bool IsSuccess => Error is null;
  public DomainError? Error { get; private init; }

  protected Result(DomainError? error) => Error = error;

  public static implicit operator Result(DomainError error) => new(error);
}

public class Result<TValue> : Result where TValue : notnull {
  private readonly TValue? _value;

  public TValue Value => _value ??
    throw new InvalidOperationException("The value of a failure result can not be accessed.");

  private Result(TValue? value, DomainError? error) : base(error) {
    _value = value;
  }

  public static implicit operator Result<TValue>(TValue value) => new(value, null);
  public static implicit operator Result<TValue>(DomainError error) => new(default, error);
}
