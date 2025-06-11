using FluentValidation;

namespace SimplifiedDnd.Application.Characters.CreateCharacter;

internal sealed class CreateCharacterCommandValidator : AbstractValidator<CreateCharacterCommand> {
  public CreateCharacterCommandValidator() {
    RuleFor(command => command.Name)
      .Must(name => !string.IsNullOrWhiteSpace(name))
      .WithMessage("Name cannot be empty");
    
    RuleFor(command => command.PlayerName)
      .Must(playerName => !string.IsNullOrWhiteSpace(playerName))
      .WithMessage("Player name cannot be empty");
    
    RuleFor(command => command.SpecieName)
      .Must(specieName => !string.IsNullOrWhiteSpace(specieName))
      .WithMessage("The name of the specie cannot be empty");

    RuleFor(command => command.Classes)
      .Must(CreateCharacterCommand.ClassesAreValid)
      .WithMessage("Must have at least one valid class");
  }
}