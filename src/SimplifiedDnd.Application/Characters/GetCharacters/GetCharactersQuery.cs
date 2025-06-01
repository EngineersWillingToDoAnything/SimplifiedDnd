using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.Application.Abstractions.Core;
using SimplifiedDnd.Application.Abstractions.Queries;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.Application.Characters.GetCharacters;

public record GetCharactersQuery : IQuery<PaginatedResult<Character>> {
  public Page? Page { get; init; }
  public Order? Order { get; init; }
  public CharacterFilter Filter { get; init; } = new();

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