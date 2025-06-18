namespace SimplifiedDnd.Domain.Characters;

public class Character {
  public required Guid Id { get; init; }
  public required string Name { get; init; }
  public required string PlayerName { get; init; }
  public Dictionary<StatType, Stat> Stats { get; init; }

  public Specie? Specie { get; init; }
  public DndClass? MainClass { get; init; }
  public IReadOnlyCollection<DndClass> Classes { get; set; } = [];

  /// <summary>
  /// Initializes a new instance of the <see cref="Character"/> class with the specified stats or with default stats for all six standard stat types.
  /// </summary>
  /// <param name="stats">
  /// An optional dictionary mapping <see cref="StatType"/> to <see cref="Stat"/>. If provided, it must contain exactly six entries for the standard stat types; otherwise, an <see cref="ArgumentException"/> is thrown.
  /// </param>
  /// <exception cref="ArgumentException">
  /// Thrown if <paramref name="stats"/> is provided and does not contain exactly six entries.
  /// </exception>
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
