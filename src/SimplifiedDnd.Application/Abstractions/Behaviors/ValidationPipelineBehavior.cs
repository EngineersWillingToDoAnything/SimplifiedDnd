using System.Reflection;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using SimplifiedDnd.Application.Abstractions.Core;

namespace SimplifiedDnd.Application.Abstractions.Behaviors;

internal sealed class ValidationPipelineBehavior<TRequest, TResponse>(
  IEnumerable<IValidator<TRequest>> validators
) : IPipelineBehavior<TRequest, TResponse>
  where TRequest : class {
  /// <summary>
  /// Handles a request by validating it with all registered validators before invoking the next handler in the pipeline.
  /// </summary>
  /// <param name="request">The request to validate and process.</param>
  /// <param name="next">The delegate representing the next handler in the pipeline.</param>
  /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
  /// <returns>
  /// The response from the next handler if validation succeeds; otherwise, a failure result containing validation errors or a thrown <see cref="ValidationException"/> if the response type is not a <c>Result</c>.
  /// </returns>
  public async Task<TResponse> Handle(
    TRequest request,
    RequestHandlerDelegate<TResponse> next,
    CancellationToken cancellationToken
  ) {
    ValidationFailure[] validationFailures = await ValidateAsync(request);

    if (validationFailures.Length == 0) {
      return await next(cancellationToken);
    }

    if (typeof(TResponse).IsGenericType &&
        typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>)) {
      Type resultType = typeof(TResponse).GetGenericArguments()[0];

      MethodInfo? failureMethod = typeof(Result)
        .GetMethod(nameof(Result.TypedFailure))
        ?.MakeGenericMethod(resultType);

      if (failureMethod is not null) {
        return (TResponse)failureMethod.Invoke(
          null,
          [CreateValidationError(validationFailures)])!;
      }
    } else if (typeof(TResponse) == typeof(Result)) {
      return (TResponse)(object)Result.Failure(CreateValidationError(validationFailures));
    }

    throw new ValidationException(validationFailures);
  }

  /// <summary>
  /// Asynchronously validates the given request using all registered validators and returns any validation failures.
  /// </summary>
  /// <param name="request">The request object to validate.</param>
  /// <returns>An array of <see cref="ValidationFailure"/> objects representing all validation errors, or an empty array if there are none.</returns>
  private async Task<ValidationFailure[]> ValidateAsync(TRequest request) {
    if (!validators.Any()) {
      return [];
    }

    var context = new ValidationContext<TRequest>(request);

    ValidationResult[] validationResults = await Task.WhenAll(
      validators.Select(validator => validator.ValidateAsync(context)));

    ValidationFailure[] validationFailures = validationResults
      .Where(validationResult => !validationResult.IsValid)
      .SelectMany(validationResult => validationResult.Errors)
      .ToArray();

    return validationFailures;
  }

  /// <summary>
    /// Creates a <see cref="ValidationError"/> containing domain errors for each validation failure.
    /// </summary>
    /// <param name="validationFailures">An array of validation failures to convert into domain errors.</param>
    /// <returns>A <see cref="ValidationError"/> representing all provided validation failures.</returns>
    private static ValidationError CreateValidationError(ValidationFailure[] validationFailures) =>
    new(validationFailures.Select(f => DomainError.Problem(f.ErrorCode, f.ErrorMessage)).ToList());
}