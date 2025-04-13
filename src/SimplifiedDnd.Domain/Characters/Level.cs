namespace SimplifiedDnd.Domain.Characters;

public class Level {
  public static Level MaxLevel => new(20, 0);

  public uint Value { get; init; }
  public uint CurrentExperience { get; set; }
  private uint _requiredExperience { get; init; }

  public bool IsMaxLevel => Value == MaxLevel.Value;

  public Level(uint value, uint currentExperience = 0) {
    if (value > MaxLevel.Value) {
      throw new ArgumentException($"Max level is {MaxLevel.Value}");
    }

    Value = value;
    if (IsMaxLevel) { return; }

    CurrentExperience = currentExperience;
    _requiredExperience = CalculateRequiredExperience();
  }

  public Level AddExperience(uint experience) {
    if (IsMaxLevel) { return MaxLevel; }

    CurrentExperience += experience;
    if (CurrentExperience < _requiredExperience) {
      return this;
    }

    return new Level(
      Value + 1, CurrentExperience - _requiredExperience);
  }

  private uint CalculateRequiredExperience() => Value * 100;
}