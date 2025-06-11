using System.Diagnostics;

namespace SimplifiedDnd.Domain.Characters;

public class Level {
  private const uint MaxValue = 20;
  private const uint MinValue = 1;
  public static readonly Level MaxLevel = new(MaxValue, 0);
  public static readonly Level MinLevel = new(MinValue, 0);

  public uint Value { get; private init; }
  public uint CurrentExperience { get; set; }
  private uint _requiredExperience { get; init; }

  public bool IsMaxLevel => Value == MaxValue;

  public Level(uint value, uint currentExperience = 0) {
    Debug.Assert(value is >= MinValue and <= MaxValue);

    Value = value;
    if (IsMaxLevel) { return; }

    CurrentExperience = currentExperience;
    _requiredExperience = CalculateRequiredExperience();
  }

  public Level AddExperience(uint experience) {
    if (IsMaxLevel) { return this; }

    CurrentExperience += experience;
    if (CurrentExperience < _requiredExperience) {
      return this;
    }

    return new Level(
      Value + 1, CurrentExperience - _requiredExperience);
  }

  private uint CalculateRequiredExperience() => Value * 100;
}