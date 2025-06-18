using System.Diagnostics.CodeAnalysis;

namespace SimplifiedDnd.Application.Abstractions.Core;

public class Result {
  [MemberNotNullWhen(false, nameof(Error))]
  public bool IsSuccess => Error is null;
  
  public DomainError? Error { get; private init; }

  /// <summary>
/// Initializes a new instance of the <see cref="Result"/> class with the specified error state.
/// </summary>
/// <param name="error">The error describing the failure, or <c>null</c> to indicate success.</param>
protected Result(DomainError? error) => Error = error;

  public static implicit operator Result(DomainError error) => new(error);

  /// <summary>
/// Creates a failure <see cref="Result"/> with the specified error.
/// </summary>
/// <param name="error">The error describing the reason for failure.</param>
/// <returns>A <see cref="Result"/> instance representing a failed operation.</returns>
public static Result Failure(DomainError error) => new(error);
  
  /// <summary>
    /// Creates a failure result of type <typeparamref name="TValue"/> with the specified error.
    /// </summary>
    /// <typeparam name="TValue">The type of the value that would be returned on success.</typeparam>
    /// <param name="error">The error describing the reason for failure.</param>
    /// <returns>A failure <see cref="Result{TValue}"/> containing the provided error and a default value.</returns>
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