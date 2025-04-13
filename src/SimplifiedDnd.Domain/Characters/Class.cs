namespace SimplifiedDnd.Domain.Characters;

public class Class {
  public required string Name { get; init; }
  public required Level Level { get; init; }

  public List<ClassStage> Stages { get; init; } = [];
}