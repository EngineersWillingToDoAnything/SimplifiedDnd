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

  public static bool IsInValidRange(int level) {
    return level is >= MinValue and <= MaxValue;
  }

  public static bool IsInValidRange(Level? level) {
    return level is not null && IsInValidRange(level.Value);
  }
  
  public Level(int value, int currentExperience = 0) {
    Value = value;
    if (IsMaxLevel) { return; }

    CurrentExperience = currentExperience;
    _requiredExperience = CalculateRequiredExperience();
  }

  public Level AddExperience(int experience) {
    if (IsMaxLevel) { return this; }

    CurrentExperience += experience;
    if (CurrentExperience < _requiredExperience) {
      return this;
    }

    return new Level(
      Value + 1, CurrentExperience - _requiredExperience);
  }

  private int CalculateRequiredExperience() => Value * 100;
}