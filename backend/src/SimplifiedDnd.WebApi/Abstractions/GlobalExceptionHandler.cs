using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using SimplifiedDnd.Application.Abstractions.Core;

namespace SimplifiedDnd.WebApi.Abstractions;

internal sealed class GlobalExceptionHandler(
  ILogger<GlobalExceptionHandler> logger
) : IExceptionHandler {
  private readonly DomainError _defaultError = new("Error.Default", "Server error", ErrorType.Failure);

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