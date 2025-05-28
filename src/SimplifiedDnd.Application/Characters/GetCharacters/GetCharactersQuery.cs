using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.Application.Abstractions.Core;
using SimplifiedDnd.Application.Abstractions.Queries;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.Application.Characters.GetCharacters;

public record GetCharactersQuery : IQuery<PaginatedResult<Character>> {
  internal Page? Page { get; init; }
  internal Order? Order { get; init; }
  internal CharacterFilter Filter { get; init; } = new();

  public bool OrderIsValid() {
    List<string> validOrderKeys = [
      nameof(Character.Id),
      nameof(Character.Name),
      nameof(Character.MainClass),
      nameof(Character.Specie)
    ];
    return Order is null ||
           validOrderKeys.Contains(Order.Key, StringComparer.OrdinalIgnoreCase);
  }
  
  public bool PageIsValid() {
    return Page is null || Page.IsValid();
  }
}