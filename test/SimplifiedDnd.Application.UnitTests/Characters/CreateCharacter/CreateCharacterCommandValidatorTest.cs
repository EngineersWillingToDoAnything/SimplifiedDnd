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
      .WithErrorMessage("Name cannot be empty")
      .WithErrorCode("CreateCharacterError.EmptyName");
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
      .WithErrorMessage("Player name cannot be empty")
      .WithErrorCode("CreateCharacterError.EmptyPlayerName");
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
      .WithErrorMessage("The name of the specie cannot be empty")
      .WithErrorCode("CreateCharacterError.EmptySpecieName");
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
      .WithErrorMessage("All classes must have a non empty name and " +
                        $"a level between {Level.MinLevel.Value} and {Level.MaxLevel.Value}")
      .WithErrorCode("CreateCharacterError.InvalidClasses");
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
      .WithErrorMessage("All classes must have a non empty name and " +
                        $"a level between {Level.MinLevel.Value} and {Level.MaxLevel.Value}")
      .WithErrorCode("CreateCharacterError.InvalidClasses");
  }
  
  [Fact(DisplayName = "Returns invalid with class with level below minimum level")]
  public void ValidatorReturnsInvalidWithClassWithLevelBelowMinimumLevel() {
    // Arrange
    var command = new CreateCharacterCommand {
      Name = "-",
      PlayerName = "-",
      SpecieName = "-",
      Classes = [new DndClass { Name = "-", Level = new Level(Level.MinLevel.Value - 1) }],
    };

    // Act
    TestValidationResult<CreateCharacterCommand>? result = _validator.TestValidate(command);

    // Assert
    result.ShouldHaveValidationErrorFor(c => c.Classes)
      .WithErrorMessage("All classes must have a non empty name and " +
                        $"a level between {Level.MinLevel.Value} and {Level.MaxLevel.Value}")
      .WithErrorCode("CreateCharacterError.InvalidClasses");
  }
  
  [Fact(DisplayName = "Returns invalid with class with level above maximum level")]
  public void ValidatorReturnsInvalidWithClassWithLevelAboveMaximumLevel() {
    // Arrange
    var command = new CreateCharacterCommand {
      Name = "-",
      PlayerName = "-",
      SpecieName = "-",
      Classes = [new DndClass { Name = "-", Level = new Level(Level.MaxLevel.Value + 1) }],
    };

    // Act
    TestValidationResult<CreateCharacterCommand>? result = _validator.TestValidate(command);

    // Assert
    result.ShouldHaveValidationErrorFor(c => c.Classes)
      .WithErrorMessage("All classes must have a non empty name and " +
                        $"a level between {Level.MinLevel.Value} and {Level.MaxLevel.Value}")
      .WithErrorCode("CreateCharacterError.InvalidClasses");
  }
  
  [Fact(DisplayName = "Returns invalid with at least one invalid class")]
  public void ValidatorReturnsInvalidWithAtLeastOneInvalidClass() {
    // Arrange
    var command = new CreateCharacterCommand {
      Name = "-",
      PlayerName = "-",
      SpecieName = "-",
      Classes = [
        new DndClass { Name = string.Empty, Level = Level.MinLevel },
        new DndClass { Name = "-", Level = Level.MinLevel }
      ],
    };

    // Act
    TestValidationResult<CreateCharacterCommand>? result = _validator.TestValidate(command);

    // Assert
    result.ShouldHaveValidationErrorFor(c => c.Classes)
      .WithErrorMessage("All classes must have a non empty name and " +
                        $"a level between {Level.MinLevel.Value} and {Level.MaxLevel.Value}")
      .WithErrorCode("CreateCharacterError.InvalidClasses");
  }
  
  [Fact(DisplayName = "Returns invalid with repeated classes")]
  public void ValidatorReturnsInvalidWithRepeatedClasses() {
    // Arrange
    const string className = "bard";
    var command = new CreateCharacterCommand {
      Name = "-",
      PlayerName = "-",
      SpecieName = "-",
      Classes = [
        new DndClass { Name = className, Level = Level.MinLevel },
        new DndClass { Name = className, Level = Level.MinLevel }
      ],
    };

    // Act
    TestValidationResult<CreateCharacterCommand>? result = _validator.TestValidate(command);

    // Assert
    result.ShouldHaveValidationErrorFor(c => c.Classes)
      .WithErrorMessage("Class names must be unique")
      .WithErrorCode("CreateCharacterError.NonUniqueClasses");
  }

  [Fact(DisplayName = "Returns valid with valid command")]
  public void ValidatorReturnsValidWithValidCommand() {
    // Arrange
    var command = new CreateCharacterCommand {
      Name = "The Best",
      PlayerName = "Me",
      SpecieName = "human",
      Classes = [
        new DndClass { Name = "artificer", Level = Level.MinLevel },
        new DndClass { Name = "bard", Level = Level.MaxLevel },
      ],
    };

    // Act
    TestValidationResult<CreateCharacterCommand>? result = _validator.TestValidate(command);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }
}