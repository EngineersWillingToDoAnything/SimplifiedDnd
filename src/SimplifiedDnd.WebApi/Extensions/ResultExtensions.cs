using SimplifiedDnd.Application.Abstractions.Core;

namespace SimplifiedDnd.WebApi.Extensions;

internal static class ResultExtensions {
  internal static TOut Match<TOut>(
    this Result result,
    Func<TOut> onSuccess,
    Func<Result, TOut> onFailure) {
    return result.IsSuccess ? onSuccess() : onFailure(result);
  }

  internal static TOut Match<TIn, TOut>(
    this Result<TIn> result,
    Func<TIn, TOut> onSuccess,
    Func<Result<TIn>, TOut> onFailure
  ) where TIn : notnull {
    return result.IsSuccess ? onSuccess(result.Value) : onFailure(result);
  }

  internal static Result<TOut> Bind<TIn, TOut>(
    this Result<TIn> result,
    Func<TIn, TOut> onSuccess
  ) where TIn : notnull
    where TOut : notnull {
    return result.IsSuccess ? onSuccess(result.Value) : result.Error;
  }
}