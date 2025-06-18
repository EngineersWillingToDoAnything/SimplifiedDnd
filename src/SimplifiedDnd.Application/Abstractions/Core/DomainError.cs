namespace SimplifiedDnd.Application.Abstractions.Core;

public record DomainError(string Code, string Description, ErrorType Type) {
  public Exception? Exception { get; set; }
  
  public static DomainError Failure(string code, string description) =>
    new(code, description, ErrorType.Failure);

  public static DomainError NotFound(string code, string description) =>
    new(code, description, ErrorType.NotFound);

  public static DomainError Problem(string code, string description) =>
    new(code, description, ErrorType.Problem);

  public static DomainError Conflict(string code, string description) =>
    new(code, description, ErrorType.Conflict);
}