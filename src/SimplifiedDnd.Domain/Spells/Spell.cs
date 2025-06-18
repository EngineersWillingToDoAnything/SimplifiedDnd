namespace SimplifiedDnd.Domain.Spells;

public class Spell {
  public required string Name { get;set; }
  public required string Description { get;set; }
  public required string Range { get;set; }
  public required uint Level { get;set; }
    
  public required SpellSchool School { get;set; }
  public required CastingTime CastingTime { get;set; }
  public SpellComponent SpellComponents { get;set; }

  /// <summary>
  /// Initializes a new instance of the <see cref="Spell"/> class with the specified spell components.
  /// </summary>
  /// <param name="component">The components required to cast the spell.</param>
  public Spell(SpellComponent component)
  {
    SpellComponents = component;
  }
}