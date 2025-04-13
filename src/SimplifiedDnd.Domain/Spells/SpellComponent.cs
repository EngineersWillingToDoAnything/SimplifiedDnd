namespace SimplifiedDnd.Domain.Spells;

[Flags]
public enum SpellComponent {
  None = 0,
  Verbal = 1,
  Somatic = 2,
  Material = 4
}