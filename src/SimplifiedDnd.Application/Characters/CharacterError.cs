using SimplifiedDnd.Application.Abstractions.Core;

namespace SimplifiedDnd.Application.Characters;

public record CharacterError(string Code, string Description) : DomainError(Code, Description) {
  public static readonly CharacterError AlreadyExists = new(
    "CharacterError.AlreadyExists",
    "A character with the same name and player name already exists.");

  public static readonly CharacterError NonExistingSpecie = new(
    "CharacterError.NonExistingSpecie",
    "The specified specie doesn't exist.");
  
  public static readonly CharacterError NonExistingClass = new(
    "CharacterError.NonExistingClass",
    "The specified class doesn't exist.");
}