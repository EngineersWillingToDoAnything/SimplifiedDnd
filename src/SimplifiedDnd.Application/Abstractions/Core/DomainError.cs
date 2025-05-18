namespace SimplifiedDnd.Application.Abstractions.Core;

public record DomainError(string Code, string Description) {
  public Exception? Exception { get; set; }
}