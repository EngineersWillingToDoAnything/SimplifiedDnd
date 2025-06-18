namespace SimplifiedDnd.Application.Abstractions.Queries;

public record Order {
  public string Key { get; private init; }
  public bool Ascending { get; private init; }

  /// <summary>
  /// Initializes a new instance of the <see cref="Order"/> record with the specified sort key and direction.
  /// </summary>
  /// <param name="key">The field name to sort by.</param>
  /// <param name="ascending">True for ascending order; false for descending.</param>
  private Order(string key, bool ascending) {
    Key = key;
    Ascending = ascending;
  }

  /// <summary>
/// Creates an <see cref="Order"/> instance with the specified key and descending sort order.
/// </summary>
/// <param name="key">The field name to sort by.</param>
/// <returns>An <see cref="Order"/> configured for descending order.</returns>
public static Order CreateDescending(string key) => new(key, false);
  /// <summary>
/// Creates an <see cref="Order"/> instance with the specified key and ascending sort direction.
/// </summary>
/// <param name="key">The field name to sort by.</param>
/// <returns>An <see cref="Order"/> representing ascending order for the given key.</returns>
public static Order CreateAscending(string key) => new(key, true);
}