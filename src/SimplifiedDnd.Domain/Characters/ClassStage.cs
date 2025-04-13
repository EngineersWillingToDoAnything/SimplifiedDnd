namespace SimplifiedDnd.Domain.Characters;

public class ClassStage {
  public required uint ProficiencyBonus { get; init; }
  public List<Feature> Features { get; init; } = [];
}