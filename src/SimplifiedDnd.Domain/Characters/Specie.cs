using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.Domain;

public class Specie {
  public required string Name {get;init;}
  public required uint Speed {get;init;}
  public required Size Size { get; init; }
  public List<Feature> Features { get; init; } = [];
}