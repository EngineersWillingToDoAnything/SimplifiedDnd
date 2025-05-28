namespace SimplifiedDnd.Application.Abstractions.Characters;

public class CharacterFilter {
  public string? Name { get; init; }
  public IReadOnlyList<string> Classes { get; init; } = [];
  public IReadOnlyList<string> Species { get; init; } = [];
}