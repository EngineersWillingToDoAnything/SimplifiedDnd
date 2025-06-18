namespace SimplifiedDnd.Domain.Spells;

public class Spell {
  public required string Name { get;set; }
  public required string Description { get;set; }
  public required string Range { get;set; }
  public required uint Level { get;set; }
    
  public required SpellSchool School { get;set; }
  public required CastingTime CastingTime { get;set; }
  public SpellComponent SpellComponents { get;set; }

  public Spell(SpellComponent component)
  {
    SpellComponents = component;
  }
}