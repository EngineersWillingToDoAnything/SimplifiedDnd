using System.Collections.ObjectModel;

namespace SimplifiedDnd.Domain.Characters;

public class DndClass {
  public required string Name { get; init; }
  public Level Level { get; init; } = Level.MinLevel;

  public Collection<ClassStage> Stages { get; init; } = [];
}