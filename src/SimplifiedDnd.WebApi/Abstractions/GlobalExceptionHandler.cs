using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using SimplifiedDnd.Application.Abstractions.Core;

namespace SimplifiedDnd.WebApi.Abstractions;

internal sealed class GlobalExceptionHandler(
  ILogger<GlobalExceptionHandler> logger
) : IExceptionHandler {
  private readonly DomainError _defaultError = new("Error.Default", "Server error", ErrorType.Failure);

  /// <summary>
  /// Handles an unhandled exception by logging it and returning a standardized JSON error response.
  /// </summary>
  /// <param name="httpContext">The current HTTP context for the request.</param>
  /// <param name="exception">The exception to handle.</param>
  /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
  /// <returns>True, indicating the exception was handled.</returns>
  public async ValueTask<bool> TryHandleAsync(
    HttpContext httpContext,
    Exception exception,
    CancellationToken cancellationToken
  ) {
#pragma warning disable CA1848
    logger.LogError(
      exception, "Exception occurred: {Message}", exception.Message);
#pragma warning restore CA1848

    ProblemHttpResult problemDetails = CustomResults.Problem(_defaultError with {
      Exception = exception,
    });

    httpContext.Response.StatusCode = problemDetails.StatusCode;

    await httpContext.Response
      .WriteAsJsonAsync(problemDetails, cancellationToken);

    return true;
  }
}