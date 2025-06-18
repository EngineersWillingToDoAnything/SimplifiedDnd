using System.Diagnostics.CodeAnalysis;
using SimplifiedDnd.Domain.Spells;

namespace SimplifiedDnd.Domain.Characters;

public class Feature {
  public required string Name { get; init; }
  public required string Description { get; init; }
  public bool IsPassive { get; init; }
    
  public Spell? Spell { get; set; }
    
  [MemberNotNullWhen(true, nameof(Spell))]
  public bool IsSpellAssociated => Spell is not null;
}