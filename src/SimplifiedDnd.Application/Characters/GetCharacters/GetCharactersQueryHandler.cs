using System.Diagnostics;
using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.Application.Abstractions.Core;
using SimplifiedDnd.Application.Abstractions.Queries;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.Application.Characters.GetCharacters;

internal sealed class GetCharactersQueryHandler(
  IReadOnlyCharacterRepository repository
) : IQueryHandler<GetCharactersQuery, PaginatedResult<Character>> {
  /// <summary>
  /// Handles a <see cref="GetCharactersQuery"/> by retrieving a paginated and optionally filtered list of characters.
  /// </summary>
  /// <param name="request">The query containing pagination, ordering, and filtering options.</param>
  /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
  /// <returns>A result containing a paginated list of characters matching the query criteria.</returns>
  public async Task<Result<PaginatedResult<Character>>> Handle(
    GetCharactersQuery request, CancellationToken cancellationToken
  ) {
    Debug.Assert(GetCharactersQuery.OrderIsValid(request.Order));
    Debug.Assert(GetCharactersQuery.PageIsValid(request.Page));

    Order order = request.Order ?? Order.CreateAscending(nameof(Character.Id));
    Page page = request.Page ?? Page.Infinite;

    return await repository.GetCharactersAsync(
      page, order, request.Filter, cancellationToken);
  }
}