using System.Diagnostics.CodeAnalysis;

namespace SimplifiedDnd.Application.Abstractions.Core;

public class Result {
  [MemberNotNullWhen(false, nameof(Error))]
  public bool IsSuccess => Error is null;
  public Error? Error { get; private init; }
  
  protected Result(Error? error) => Error = error;

  public static implicit operator Result(Error error) => new(error);
}

public class Result<TValue> : Result where TValue : notnull {
  private readonly TValue? _value;
  
  public TValue Value => _value ??
    throw new InvalidOperationException("The value of a failure result can not be accessed.");
  
  private Result(TValue? value, Error? error) : base(error) {
    _value = value;
  }

  public static implicit operator Result<TValue>(TValue value) => new(value, null);
  public static implicit operator Result<TValue>(Error error) => new(default, error);
}
