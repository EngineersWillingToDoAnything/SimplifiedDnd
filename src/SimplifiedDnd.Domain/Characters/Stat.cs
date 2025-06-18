namespace SimplifiedDnd.Domain.Characters;

public class Stat {
  public const uint MaxValue = 18;
  public const uint DefaultValue = 8;

  public const uint AssignablePoints = 27 + DefaultValue * 6;

  // CompraDePuntos
  // TiradaDeDados
  public uint Value { get; set; }
  public int Modifier { get; private set; }

  /// <summary>
  /// Initializes a new instance of the <see cref="Stat"/> class with the specified value.
  /// </summary>
  /// <param name="value">The initial value for the stat. Must not exceed <see cref="MaxValue"/>. Defaults to <see cref="DefaultValue"/> if not specified.</param>
  /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is greater than <see cref="MaxValue"/>.</exception>
  public Stat(uint value = DefaultValue) {
    switch (value) {
      case > MaxValue:
        throw new ArgumentOutOfRangeException(
          nameof(value), value, $"Value must be less than {MaxValue}");
      default:
        Value = value;
        Modifier = CalculateModifier();
        break;
    }
  }

  /// <summary>
/// Calculates the modifier for the stat based on its current value.
/// </summary>
/// <returns>The modifier as an integer, computed as (Value / 2) - 5.</returns>
private int CalculateModifier() => (int)Value / 2 - 5;
}