using FluentValidation;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.Application.Characters.CreateCharacter;

internal sealed class CreateCharacterCommandValidator : AbstractValidator<CreateCharacterCommand> {
  /// <summary>
  /// Initializes a new instance of the <see cref="CreateCharacterCommandValidator"/> class, defining validation rules for <see cref="CreateCharacterCommand"/>.
  /// </summary>
  /// <remarks>
  /// Enforces that the character name, player name, and specie name are not empty. Validates that each class in the command has a non-empty name and a level within allowed bounds, and ensures all class names are unique (case-insensitive).
  /// </remarks>
  public CreateCharacterCommandValidator() {
    RuleFor(command => command.Name)
      .NotEmpty()
      .WithMessage("Name cannot be empty")
      .WithErrorCode("CreateCharacterError.EmptyName");

    RuleFor(command => command.PlayerName)
      .NotEmpty()
      .WithMessage("Player name cannot be empty")
      .WithErrorCode("CreateCharacterError.EmptyPlayerName");

    RuleFor(command => command.SpecieName)
      .NotEmpty()
      .WithMessage("The name of the specie cannot be empty")
      .WithErrorCode("CreateCharacterError.EmptySpecieName");

    RuleFor(command => command.Classes)
      .Must(CreateCharacterCommand.ClassesAreValid)
      .WithMessage("All classes must have a non empty name and " +
                   $"a level between {Level.MinLevel.Value} and {Level.MaxLevel.Value}")
      .WithErrorCode("CreateCharacterError.InvalidClasses");

    RuleFor(command => command.Classes)
      .Must(classes =>
        classes.Count ==
        classes.Select(c => c.Name?.ToUpperInvariant()).ToHashSet().Count)
      .WithMessage("Class names must be unique")
      .WithErrorCode("CreateCharacterError.NonUniqueClasses");
  }
}