using System.Diagnostics;

namespace SimplifiedDnd.Domain.Characters;

public class Level {
  private const int MaxValue = 20;
  private const int MinValue = 1;
  public static readonly Level MaxLevel = new(MaxValue, 0);
  public static readonly Level MinLevel = new(MinValue, 0);

  public int Value { get; private init; }
  public int CurrentExperience { get; set; }
  private int _requiredExperience { get; init; }

  public bool IsMaxLevel => Value == MaxValue;

  /// <summary>
  /// Determines whether the specified integer value is within the valid level range.
  /// </summary>
  /// <param name="level">The level value to check.</param>
  /// <returns>True if the level is between the minimum and maximum allowed values; otherwise, false.</returns>
  public static bool IsInValidRange(int level) {
    return level is >= MinValue and <= MaxValue;
  }

  /// <summary>
  /// Determines whether the specified <see cref="Level"/> instance is not null and its value is within the valid level range.
  /// </summary>
  /// <param name="level">The <see cref="Level"/> instance to validate.</param>
  /// <returns><c>true</c> if the level is not null and its value is between the minimum and maximum allowed; otherwise, <c>false</c>.</returns>
  public static bool IsInValidRange(Level? level) {
    return level is not null && IsInValidRange(level.Value);
  }
  
  /// <summary>
  /// Initializes a new instance of the Level class with the specified level value and optional current experience.
  /// </summary>
  /// <param name="value">The level value to assign.</param>
  /// <param name="currentExperience">The experience points accumulated at the current level. Defaults to 0.</param>
  public Level(int value, int currentExperience = 0) {
    Value = value;
    if (IsMaxLevel) { return; }

    CurrentExperience = currentExperience;
    _requiredExperience = CalculateRequiredExperience();
  }

  /// <summary>
  /// Adds the specified experience points to the current level and returns a new Level instance if a level-up occurs.
  /// </summary>
  /// <param name="experience">The amount of experience points to add.</param>
  /// <returns>
  /// The current Level instance if no level-up occurs or the maximum level is reached; otherwise, a new Level instance with the next level and remaining experience points.
  /// </returns>
  public Level AddExperience(int experience) {
    if (IsMaxLevel) { return this; }

    CurrentExperience += experience;
    if (CurrentExperience < _requiredExperience) {
      return this;
    }

    return new Level(
      Value + 1, CurrentExperience - _requiredExperience);
  }

  /// <summary>
/// Calculates the experience points required to advance from the current level to the next.
/// </summary>
/// <returns>The required experience points to reach the next level.</returns>
private int CalculateRequiredExperience() => Value * 100;
}