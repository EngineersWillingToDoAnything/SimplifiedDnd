namespace SimplifiedDnd.Application.Abstractions.Queries;

public class PaginatedResult<TValue> {
  public required IReadOnlyCollection<TValue> Values { get; init; }
  public required int TotalAmount { get; init; }
}