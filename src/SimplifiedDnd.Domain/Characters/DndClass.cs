using System.Collections.ObjectModel;

namespace SimplifiedDnd.Domain.Characters;

public class DndClass {
  public required string Name { get; init; }
  public required Level Level { get; init; }

  public Collection<ClassStage> Stages { get; init; } = [];
}