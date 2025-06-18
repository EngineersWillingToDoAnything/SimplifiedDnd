using SimplifiedDnd.WebApi.Filters;

namespace SimplifiedDnd.WebApi.Extensions;

internal static class ValidationExtensions {
  /// <summary>
  /// Adds request validation to the route handler by applying a validation filter for the specified request type and configuring the endpoint to produce a validation problem response.
  /// </summary>
  /// <typeparam name="TRequest">The type of the request to validate.</typeparam>
  /// <returns>The modified <see cref="RouteHandlerBuilder"/> with validation configured.</returns>
  internal static RouteHandlerBuilder WithRequestValidation<TRequest>(
    this RouteHandlerBuilder builder
  ) {
    return builder.AddEndpointFilter<ValidationFilter<TRequest>>()
      .ProducesValidationProblem();
  }
}