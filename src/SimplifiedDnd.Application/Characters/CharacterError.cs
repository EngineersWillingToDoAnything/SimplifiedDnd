using SimplifiedDnd.Application.Abstractions.Core;

namespace SimplifiedDnd.Application.Characters;

public record CharacterError(string Code, string Description) : Error(Code, Description) {
  public static readonly CharacterError NonExistingSpecie = new(
    "CharacterError.NonExistingSpecie", 
    "The specified specie doesn't exist.");
}