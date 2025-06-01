using SimplifiedDnd.Application.Abstractions.Core;

namespace SimplifiedDnd.Application.Characters;

public record CharacterError(
  string Code,
  string Description,
  ErrorType Type
) : DomainError(Code, Description, Type) {
  public static readonly CharacterError AlreadyExists = new(
    "CharacterError.AlreadyExists",
    "A character with the same name and player name already exists.",
    ErrorType.Conflict);

  public static readonly CharacterError NonExistingSpecie = new(
    "CharacterError.NonExistingSpecie",
    "The specified specie doesn't exist.",
    ErrorType.NotFound);

  public static readonly CharacterError NonExistingClass = new(
    "CharacterError.NonExistingClass",
    "The specified class doesn't exist.",
    ErrorType.NotFound);
}