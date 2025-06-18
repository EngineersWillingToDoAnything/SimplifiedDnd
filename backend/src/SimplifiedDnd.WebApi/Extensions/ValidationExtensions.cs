using SimplifiedDnd.WebApi.Filters;

namespace SimplifiedDnd.WebApi.Extensions;

internal static class ValidationExtensions {
  internal static RouteHandlerBuilder WithRequestValidation<TRequest>(
    this RouteHandlerBuilder builder
  ) {
    return builder.AddEndpointFilter<ValidationFilter<TRequest>>()
      .ProducesValidationProblem();
  }
}