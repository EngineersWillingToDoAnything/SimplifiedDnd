namespace SimplifiedDnd.Domain.Characters;

public class Stat {
  public const uint MaxValue = 18;
  public const uint DefaultValue = 8;
  public const uint AssignablePoints = 27 + DefaultValue * 6;
      // CompraDePuntos
      // TiradaDeDados
  public uint Value { get; set; }
  public int Modifier { get; private set; }

  public Stat(uint value = DefaultValue)
  {
    switch (value)
    {
      case > MaxValue:
        throw new ArgumentOutOfRangeException(
          nameof(value), value, $"Value must be less than {MaxValue}");
      default:
        Value = value;
        Modifier = CalculateModifier();
        break;
    }
  }
  
  private int CalculateModifier() => (int)Value / 2 - 5;
}