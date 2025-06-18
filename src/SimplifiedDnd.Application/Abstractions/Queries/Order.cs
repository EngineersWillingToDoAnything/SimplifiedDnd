namespace SimplifiedDnd.Application.Abstractions.Queries;

public record Order {
  public string Key { get; private init; }
  public bool Ascending { get; private init; }

  private Order(string key, bool ascending) {
    Key = key;
    Ascending = ascending;
  }

  public static Order CreateDescending(string key) => new(key, false);
  public static Order CreateAscending(string key) => new(key, true);
}