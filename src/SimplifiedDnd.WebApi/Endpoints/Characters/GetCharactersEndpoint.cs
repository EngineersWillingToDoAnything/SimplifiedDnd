using MediatR;
using Microsoft.AspNetCore.Mvc;
using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.Application.Abstractions.Core;
using SimplifiedDnd.Application.Abstractions.Queries;
using SimplifiedDnd.Application.Characters.GetCharacters;
using SimplifiedDnd.Domain.Characters;
using SimplifiedDnd.WebApi.Abstractions;
using SimplifiedDnd.WebApi.Extensions;

namespace SimplifiedDnd.WebApi.Endpoints.Characters;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class GetCharactersEndpoint : IEndpoint {
  private sealed record Request {
    private Page? _page;
    private Order? _order;
    private CharacterFilter _filter = new();

    internal Request WithPage(int? pageIndex, int? pageSize) {
      if (pageIndex is not null && pageSize is not null) {
        _page = new Page(pageIndex.Value, pageSize.Value);
      }

      return this;
    }

    internal Request WithOrder(bool? sortAscending, string? orderKey) {
      if (sortAscending is not null && orderKey is not null) {
        _order = sortAscending.Value ? Order.CreateAscending(orderKey) : Order.CreateDescending(orderKey);
      }

      return this;
    }

    internal Request WithFilter(
      string? partOfCharacterName,
      string? possibleSpecies,
      string? possibleClasses
    ) {
      _filter = new CharacterFilter {
        Name = partOfCharacterName,
        Species = [..ParseCustomList(possibleSpecies)],
        Classes = [..ParseCustomList(possibleClasses)]
      };
      return this;
    }

    internal GetCharactersQuery ToQuery() {
      return new GetCharactersQuery {
        Page = _page,
        Order = _order,
        Filter = _filter
      };
    }

    private static IEnumerable<string> ParseCustomList(string? list) {
      const char customListSeparator = ',';
      return list?.Split(customListSeparator)
        .Select(s => s.Trim())
        .Where(s => !string.IsNullOrWhiteSpace(s)) ?? [];
    }
  }

  // ReSharper disable once NotAccessedPositionalProperty.Global
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
    [FromQuery(Name = "filter-name")] string? partOfCharacterName,
    [FromQuery(Name = "filter-species")] string? possibleSpecies,
    [FromQuery(Name = "filter-classes")] string? possibleClasses,
    ISender sender,
    CancellationToken cancellationToken
  ) {
    GetCharactersQuery query = new Request()
      .WithPage(pageIndex, pageSize)
      .WithOrder(ascending, orderKey)
      .WithFilter(partOfCharacterName, possibleSpecies, possibleClasses)
      .ToQuery();

    Result<PaginatedResult<Character>> response = await sender.Send(
      query, cancellationToken);

    return response
      .Bind(Response.FromResult)
      .Match(Results.Ok, CustomResults.Problem);
  }
}