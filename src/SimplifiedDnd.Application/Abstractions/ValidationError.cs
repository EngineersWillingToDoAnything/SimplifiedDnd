using SimplifiedDnd.Application.Abstractions.Core;

namespace SimplifiedDnd.Application.Abstractions;

public sealed record ValidationError(
  IReadOnlyCollection<DomainError> Errors
) : DomainError("Validation.General",
  "One or more validation errors occurred",
  ErrorType.Validation);