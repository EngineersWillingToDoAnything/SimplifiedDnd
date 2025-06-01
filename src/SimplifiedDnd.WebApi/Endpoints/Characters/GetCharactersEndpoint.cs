using MediatR;
using Microsoft.AspNetCore.Mvc;
using SimplifiedDnd.Application.Abstractions.Core;
using SimplifiedDnd.Application.Abstractions.Queries;
using SimplifiedDnd.Application.Characters.GetCharacters;
using SimplifiedDnd.Domain.Characters;
using SimplifiedDnd.WebApi.Abstractions;
using SimplifiedDnd.WebApi.Extensions;

namespace SimplifiedDnd.WebApi.Endpoints.Characters;

internal sealed class GetCharactersEndpoint : IEndpoint {
  private sealed record Request(
    int? PageIndex,
    int? PageSize,
    bool? SortAscending,
    string? OrderKey
  ) {
    internal GetCharactersQuery ToQuery() {
      Page? page = null;

      if (PageIndex is not null && PageSize is not null) {
        page = new Page(PageIndex.Value, PageSize.Value);
      }

      Order? order = null;
      if (SortAscending is not null && OrderKey is not null) {
        order = SortAscending.Value ? Order.CreateAscending(OrderKey) : Order.CreateDescending(OrderKey);
      }

      return new GetCharactersQuery {
        Page = page,
        Order = order
      };
    }
  }

  internal sealed record Response(string Name) {
    internal static IReadOnlyCollection<Response> FromResult(
      PaginatedResult<Character> result
    ) {
      return [..result.Values.Select(x => new Response(x.Name))];
    }
  }

  public void MapEndpoint(IEndpointRouteBuilder app) {
    app.MapGet("api/characters", Handle)
      .WithTags(Tags.Characters);
  }

  private static async Task<IResult> Handle(
    [FromQuery(Name = "page-index")] int? pageIndex,
    [FromQuery(Name = "page-size")] int? pageSize,
    [FromQuery(Name = "order-asc")] bool? ascending,
    [FromQuery(Name = "order-key")] string? orderKey,
    ISender sender,
    CancellationToken cancellationToken
  ) {
    GetCharactersQuery query = new Request(pageIndex, pageSize, ascending, orderKey).ToQuery();

    Result<PaginatedResult<Character>> response = await sender.Send(
      query, cancellationToken);

    return response
      .Bind(Response.FromResult)
      .Match(Results.Ok, CustomResults.Problem);
  }
}