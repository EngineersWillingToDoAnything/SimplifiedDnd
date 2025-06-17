namespace SimplifiedDnd.Domain.Characters;

public class Character {
  public required Guid Id { get; init; }
  public required string Name { get; init; }
  public required string PlayerName { get; init; }
  public Dictionary<StatType, Stat> Stats { get; init; }

  public Specie? Specie { get; init; }
  public DndClass? MainClass { get; init; }
  public IReadOnlyCollection<DndClass> Classes { get; set; } = [];

  public Character(Dictionary<StatType, Stat>? stats = null) {
    if (stats is not null && stats.Count != 6) {
      throw new ArgumentException("Invalid character stats");
    }

    Stats = stats ?? new Dictionary<StatType, Stat>() {
      { StatType.Strength, new Stat() },
      { StatType.Dexterity, new Stat() },
      { StatType.Constitution, new Stat() },
      { StatType.Intelligence, new Stat() },
      { StatType.Wisdom, new Stat() },
      { StatType.Charisma, new Stat() },
    };
  }
}
