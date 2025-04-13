using System.Collections.ObjectModel;

namespace SimplifiedDnd.Domain.Characters;

public class Specie {
  public required string Name { get; init; }
  public required uint Speed { get; init; }
  public required Size Size { get; init; }
  public Collection<Feature> Features { get; init; } = [];
}