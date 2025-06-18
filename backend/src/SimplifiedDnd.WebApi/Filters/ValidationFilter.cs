using FluentValidation;
using FluentValidation.Results;

namespace SimplifiedDnd.WebApi.Filters;

internal class ValidationFilter<TRequest>(
  IValidator<TRequest> validator
) : IEndpointFilter {
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