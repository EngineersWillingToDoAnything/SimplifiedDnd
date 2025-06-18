using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.Application.Abstractions.Core;
using SimplifiedDnd.Application.Abstractions.Queries;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.Application.Characters.GetCharacters;

public record GetCharactersQuery : IQuery<PaginatedResult<Character>> {
  public Page? Page { get; init; }
  public Order? Order { get; init; }
  public CharacterFilter Filter { get; init; } = new();

  /// <summary>
  /// Determines whether the provided order key is valid for character queries.
  /// </summary>
  /// <param name="order">The order parameter to validate.</param>
  /// <returns>True if the order is null or its key matches a valid character property; otherwise, false.</returns>
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

  /// <summary>
  /// Determines whether the specified page parameter is either null or represents a valid page.
  /// </summary>
  /// <param name="page">The page parameter to validate.</param>
  /// <returns>True if the page is null or valid; otherwise, false.</returns>
  public static bool PageIsValid(Page? page) {
    return page is null || page.IsValid();
  }
}