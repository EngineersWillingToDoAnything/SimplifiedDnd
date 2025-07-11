using Microsoft.AspNetCore.Http.HttpResults;
using SimplifiedDnd.Application.Abstractions;
using SimplifiedDnd.Application.Abstractions.Core;

namespace SimplifiedDnd.WebApi.Abstractions;

internal static class CustomResults {
  internal static ProblemHttpResult Problem(Result result) {
    if (result.IsSuccess) {
      throw new InvalidOperationException();
    }

    return TypedResults.Problem(
      title: GetTitle(result.Error),
      detail: GetDetail(result.Error),
      type: GetType(result.Error.Type),
      statusCode: GetStatusCode(result.Error.Type),
      extensions: GetErrors(result));

    static string GetTitle(DomainError error) =>
      error.Type switch {
        ErrorType.Validation or
          ErrorType.Problem or
          ErrorType.NotFound or
          ErrorType.Conflict => error.Code,
        _ => "Server failure"
      };

    static string GetDetail(DomainError error) =>
      error.Type switch {
        ErrorType.Validation or
          ErrorType.Problem or
          ErrorType.NotFound or
          ErrorType.Conflict => error.Description,
        _ => "An unexpected error occurred"
      };

    static string GetType(ErrorType errorType) =>
      errorType switch {
        ErrorType.Validation => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        ErrorType.Problem => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        ErrorType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
        ErrorType.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
        _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
      };

    static int GetStatusCode(ErrorType errorType) =>
      errorType switch {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status500InternalServerError
      };

    static Dictionary<string, object?>? GetErrors(Result result) {
      if (result.Error is not ValidationError validationError) {
        return null;
      }

      return new Dictionary<string, object?> {
        { "errors", validationError.Errors.ToArray() }
      };
    }
  }
}