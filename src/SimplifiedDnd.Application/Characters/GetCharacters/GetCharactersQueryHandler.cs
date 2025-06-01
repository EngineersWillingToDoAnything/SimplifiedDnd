using System.Diagnostics;
using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.Application.Abstractions.Core;
using SimplifiedDnd.Application.Abstractions.Queries;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.Application.Characters.GetCharacters;

internal sealed class GetCharactersQueryHandler(
  IReadOnlyCharacterRepository repository
) : IQueryHandler<GetCharactersQuery, PaginatedResult<Character>> {
  public async Task<Result<PaginatedResult<Character>>> Handle(
    GetCharactersQuery request, CancellationToken cancellationToken) {
    Debug.Assert(GetCharactersQuery.OrderIsValid(request.Order));
    Debug.Assert(GetCharactersQuery.PageIsValid(request.Page));

    Order order = request.Order ?? Order.CreateAscending(nameof(Character.Id));
    Page page = request.Page ?? Page.Infinite;
    
    return await repository.GetCharactersAsync(
      page, order, request.Filter, cancellationToken);
  }
}