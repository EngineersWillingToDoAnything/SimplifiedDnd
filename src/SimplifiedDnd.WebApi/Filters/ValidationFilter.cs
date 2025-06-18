using FluentValidation;
using FluentValidation.Results;

namespace SimplifiedDnd.WebApi.Filters;

internal class ValidationFilter<TRequest>(
  IValidator<TRequest> validator
) : IEndpointFilter {
  /// <summary>
  /// Validates the incoming request using the configured validator and either continues the endpoint pipeline or returns a validation problem response.
  /// </summary>
  /// <param name="context">The invocation context containing the request and HTTP context.</param>
  /// <param name="next">The next delegate in the endpoint filter pipeline.</param>
  /// <returns>
  /// The result of the next delegate if validation succeeds; otherwise, a validation problem response containing validation errors.
  /// </returns>
  public async ValueTask<object?> InvokeAsync(
    EndpointFilterInvocationContext context, EndpointFilterDelegate next
  ) {
    TRequest request = context.Arguments.OfType<TRequest>().First();

    ValidationResult? result = await validator.ValidateAsync(
      request, context.HttpContext.RequestAborted);

    return result.IsValid
      ? await next(context)
      : TypedResults.ValidationProblem(result.ToDictionary());
  }
}