using SimplifiedDnd.Application.Abstractions.Core;

namespace SimplifiedDnd.WebApi.Extensions;

internal static class ResultExtensions {
  /// <summary>
  /// Executes the specified delegate based on whether the <see cref="Result"/> represents success or failure.
  /// </summary>
  /// <typeparam name="TOut">The type of the value to return.</typeparam>
  /// <param name="onSuccess">Delegate to invoke if the result is successful.</param>
  /// <param name="onFailure">Delegate to invoke if the result is a failure, receiving the failed result.</param>
  /// <returns>The value returned by either <paramref name="onSuccess"/> or <paramref name="onFailure"/>, depending on the result state.</returns>
  internal static TOut Match<TOut>(
    this Result result,
    Func<TOut> onSuccess,
    Func<Result, TOut> onFailure) {
    return result.IsSuccess ? onSuccess() : onFailure(result);
  }

  /// <summary>
  /// Executes one of two functions based on whether the <see cref="Result{TIn}"/> is successful, returning a value of type <typeparamref name="TOut"/>.
  /// </summary>
  /// <typeparam name="TIn">The type of the value contained in the result if successful.</typeparam>
  /// <typeparam name="TOut">The type of the value to return.</typeparam>
  /// <param name="result">The result to evaluate.</param>
  /// <param name="onSuccess">Function to execute if the result is successful, receiving the contained value.</param>
  /// <param name="onFailure">Function to execute if the result is a failure, receiving the failed result.</param>
  /// <returns>The value returned by either <paramref name="onSuccess"/> or <paramref name="onFailure"/>, depending on the result's state.</returns>
  internal static TOut Match<TIn, TOut>(
    this Result<TIn> result,
    Func<TIn, TOut> onSuccess,
    Func<Result<TIn>, TOut> onFailure
  ) where TIn : notnull {
    return result.IsSuccess ? onSuccess(result.Value) : onFailure(result);
  }

  /// <summary>
  /// Transforms the value of a successful <see cref="Result{TIn}"/> using the specified function, or propagates the error if the result is a failure.
  /// </summary>
  /// <typeparam name="TIn">The type of the value contained in the input result.</typeparam>
  /// <typeparam name="TOut">The type of the value in the returned result.</typeparam>
  /// <param name="result">The input result to bind.</param>
  /// <param name="onSuccess">A function to apply to the value if the result is successful.</param>
  /// <returns>A <see cref="Result{TOut}"/> containing the transformed value if successful, or the original error if not.</returns>
  internal static Result<TOut> Bind<TIn, TOut>(
    this Result<TIn> result,
    Func<TIn, TOut> onSuccess
  ) where TIn : notnull
    where TOut : notnull {
    return result.IsSuccess ? onSuccess(result.Value) : result.Error;
  }
}