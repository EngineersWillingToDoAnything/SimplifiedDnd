using FluentValidation.TestHelper;
using SimplifiedDnd.Application.Characters.CreateCharacter;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.Application.UnitTests.Characters.CreateCharacter;

public class CreateCharacterCommandValidatorTest {
  private readonly CreateCharacterCommandValidator _validator = new();

  private class NullOrWhiteSpaceData() : TheoryData<string?>(
    null,
    string.Empty,
    " ");

  [Theory(DisplayName = "Returns invalid with empty name")]
  [ClassData(typeof(NullOrWhiteSpaceData))]
  public void ValidatorReturnsInvalidWithEmptyName(string? name) {
    // Arrange
    var command = new CreateCharacterCommand {
      Name = name!,
      PlayerName = null!,
      SpecieName = null!,
      Classes = [],
    };

    // Act
    TestValidationResult<CreateCharacterCommand>? result = _validator.TestValidate(command);

    // Assert
    result.ShouldHaveValidationErrorFor(c => c.Name)
      .WithErrorMessage("Name cannot be empty");
  }

  [Theory(DisplayName = "Returns invalid with empty player name")]
  [ClassData(typeof(NullOrWhiteSpaceData))]
  public void ValidatorReturnsInvalidWithEmptyPlayerName(string? playerName) {
    // Arrange
    var command = new CreateCharacterCommand {
      Name = "-",
      PlayerName = playerName!,
      SpecieName = null!,
      Classes = [],
    };

    // Act
    TestValidationResult<CreateCharacterCommand>? result = _validator.TestValidate(command);

    // Assert
    result.ShouldHaveValidationErrorFor(c => c.PlayerName)
      .WithErrorMessage("Player name cannot be empty");
  }

  [Theory(DisplayName = "Returns invalid with empty specie name")]
  [ClassData(typeof(NullOrWhiteSpaceData))]
  public void ValidatorReturnsInvalidWithEmptySpecieName(string? specieName) {
    // Arrange
    var command = new CreateCharacterCommand {
      Name = "-",
      PlayerName = "-",
      SpecieName = specieName!,
      Classes = [],
    };

    // Act
    TestValidationResult<CreateCharacterCommand>? result = _validator.TestValidate(command);

    // Assert
    result.ShouldHaveValidationErrorFor(c => c.SpecieName)
      .WithErrorMessage("The name of the specie cannot be empty");
  }

  [Fact(DisplayName = "Returns invalid without classes")]
  public void ValidatorReturnsInvalidWithoutClasses() {
    // Arrange
    var command = new CreateCharacterCommand {
      Name = "-",
      PlayerName = "-",
      SpecieName = "-",
      Classes = [],
    };

    // Act
    TestValidationResult<CreateCharacterCommand>? result = _validator.TestValidate(command);

    // Assert
    result.ShouldHaveValidationErrorFor(c => c.Classes)
      .WithErrorMessage("Must have at least one valid class");
  }

  [Theory(DisplayName = "Returns invalid with class with empty name")]
  [ClassData(typeof(NullOrWhiteSpaceData))]
  public void ValidatorReturnsInvalidWithClassWithEmptyName(string? className) {
    // Arrange
    var command = new CreateCharacterCommand {
      Name = "-",
      PlayerName = "-",
      SpecieName = "-",
      Classes = [new DndClass { Name = className!, Level = Level.MinLevel }],
    };

    // Act
    TestValidationResult<CreateCharacterCommand>? result = _validator.TestValidate(command);

    // Assert
    result.ShouldHaveValidationErrorFor(c => c.Classes)
      .WithErrorMessage("Must have at least one valid class");
  }

  [Fact(DisplayName = "Returns valid with valid command")]
  public void ValidatorReturnsValidWithValidCommand() {
    // Arrange
    var command = new CreateCharacterCommand {
      Name = "-",
      PlayerName = "-",
      SpecieName = "-",
      Classes = [new DndClass { Name = "-", Level = Level.MinLevel }],
    };

    // Act
    TestValidationResult<CreateCharacterCommand>? result = _validator.TestValidate(command);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }
}