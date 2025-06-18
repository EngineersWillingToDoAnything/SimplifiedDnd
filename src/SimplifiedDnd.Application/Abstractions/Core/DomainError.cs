namespace SimplifiedDnd.Application.Abstractions.Core;

public record DomainError(string Code, string Description, ErrorType Type) {
  public Exception? Exception { get; set; }
  
  /// <summary>
    /// Creates a <see cref="DomainError"/> instance representing a failure error type.
    /// </summary>
    /// <param name="code">A unique identifier for the error.</param>
    /// <param name="description">A human-readable description of the error.</param>
    /// <returns>A <see cref="DomainError"/> with the specified code, description, and a type of <see cref="ErrorType.Failure"/>.</returns>
    public static DomainError Failure(string code, string description) =>
    new(code, description, ErrorType.Failure);

  /// <summary>
    /// Creates a <see cref="DomainError"/> instance representing a not found error with the specified code and description.
    /// </summary>
    /// <param name="code">A unique identifier for the error.</param>
    /// <param name="description">A human-readable description of the error.</param>
    /// <returns>A <see cref="DomainError"/> with <see cref="ErrorType.NotFound"/>.</returns>
    public static DomainError NotFound(string code, string description) =>
    new(code, description, ErrorType.NotFound);

  /// <summary>
    /// Creates a <see cref="DomainError"/> instance representing a problem error with the specified code and description.
    /// </summary>
    /// <param name="code">A unique identifier for the error.</param>
    /// <param name="description">A human-readable description of the error.</param>
    /// <returns>A <see cref="DomainError"/> with <see cref="ErrorType.Problem"/>.</returns>
    public static DomainError Problem(string code, string description) =>
    new(code, description, ErrorType.Problem);

  /// <summary>
    /// Creates a <see cref="DomainError"/> instance representing a conflict error with the specified code and description.
    /// </summary>
    /// <param name="code">A unique identifier for the error.</param>
    /// <param name="description">A human-readable description of the error.</param>
    /// <returns>A <see cref="DomainError"/> with <see cref="ErrorType.Conflict"/>.</returns>
    public static DomainError Conflict(string code, string description) =>
    new(code, description, ErrorType.Conflict);
}