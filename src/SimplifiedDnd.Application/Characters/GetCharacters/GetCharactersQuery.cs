using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.Application.Abstractions.Core;
using SimplifiedDnd.Application.Abstractions.Queries;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.Application.Characters.GetCharacters;

public record GetCharactersQuery : IQuery<PaginatedResult<Character>> {
  public Page? Page { get; init; }
  public Order? Order { get; init; }
  public CharacterFilter Filter { get; init; } = new();

  public static bool OrderIsValid(Order? order) {
    List<string> validOrderKeys = [
      nameof(Character.Id),
      nameof(Character.Name),
      nameof(Character.MainClass),
      nameof(Character.Specie)
    ];
    return order is null ||
           validOrderKeys.Contains(order.Key, StringComparer.OrdinalIgnoreCase);
  }

  public static bool PageIsValid(Page? page) {
    return page is null || page.IsValid();
  }
}