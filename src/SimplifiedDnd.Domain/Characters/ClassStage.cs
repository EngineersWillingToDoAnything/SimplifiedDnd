using System.Collections.ObjectModel;

namespace SimplifiedDnd.Domain.Characters;

public class ClassStage {
  public required uint ProficiencyBonus { get; init; }
  public Collection<Feature> Features { get; init; } = [];
}