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

    /// <summary>
    /// Sets the pagination parameters if both page index and page size are provided.
    /// </summary>
    /// <param name="pageIndex">The index of the page to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>The updated <see cref="Request"/> instance with pagination applied if both parameters are specified.</returns>
    internal Request WithPage(int? pageIndex, int? pageSize) {
      if (pageIndex is not null && pageSize is not null) {
        _page = new Page(pageIndex.Value, pageSize.Value);
      }

      return this;
    }

    /// <summary>
    /// Sets the ordering criteria for the request if both sort direction and order key are provided.
    /// </summary>
    /// <param name="sortAscending">Indicates whether the sorting should be ascending.</param>
    /// <param name="orderKey">The key by which to order the results.</param>
    /// <returns>The updated request with ordering applied if applicable.</returns>
    internal Request WithOrder(bool? sortAscending, string? orderKey) {
      if (sortAscending is not null && orderKey is not null) {
        _order = sortAscending.Value ? Order.CreateAscending(orderKey) : Order.CreateDescending(orderKey);
      }

      return this;
    }

    /// <summary>
    /// Sets the character filter criteria using optional name, species, and classes values.
    /// </summary>
    /// <param name="partOfCharacterName">A substring to match within character names, or null to ignore.</param>
    /// <param name="possibleSpecies">A comma-separated list of species to filter by, or null to ignore.</param>
    /// <param name="possibleClasses">A comma-separated list of classes to filter by, or null to ignore.</param>
    /// <returns>The updated <see cref="Request"/> instance with the specified filter applied.</returns>
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

    /// <summary>
    /// Converts the current request parameters into a <see cref="GetCharactersQuery"/> object for querying character data.
    /// </summary>
    /// <returns>A <see cref="GetCharactersQuery"/> populated with pagination, ordering, and filtering options.</returns>
    internal GetCharactersQuery ToQuery() {
      return new GetCharactersQuery {
        Page = _page,
        Order = _order,
        Filter = _filter
      };
    }

    /// <summary>
    /// Splits a comma-separated string into a collection of trimmed, non-empty strings.
    /// </summary>
    /// <param name="list">A comma-separated list of values, or null.</param>
    /// <returns>An enumerable of trimmed, non-empty strings parsed from the input; returns an empty collection if input is null or contains only whitespace.</returns>
    private static IEnumerable<string> ParseCustomList(string? list) {
      const char customListSeparator = ',';
      return list?.Split(customListSeparator)
        .Select(s => s.Trim())
        .Where(s => !string.IsNullOrWhiteSpace(s)) ?? [];
    }
  }

  // ReSharper disable once NotAccessedPositionalProperty.Global
  internal sealed record Response(string Name) {
    /// <summary>
    /// Converts a paginated result of <see cref="Character"/> objects into a collection of <see cref="Response"/> objects containing character names.
    /// </summary>
    /// <param name="result">The paginated result containing <see cref="Character"/> entities.</param>
    /// <returns>A read-only collection of <see cref="Response"/> objects with character names.</returns>
    internal static IReadOnlyCollection<Response> FromResult(
      PaginatedResult<Character> result
    ) {
      return [..result.Values.Select(x => new Response(x.Name))];
    }
  }

  /// <summary>
  /// Maps the HTTP GET endpoint for retrieving characters with optional pagination, ordering, and filtering.
  /// </summary>
  public void MapEndpoint(IEndpointRouteBuilder app) {
    app.MapGet("api/characters", Handle)
      .WithTags(Tags.Characters);
  }

  /// <summary>
  /// Handles HTTP GET requests to retrieve a paginated, optionally ordered and filtered list of character names.
  /// </summary>
  /// <param name="pageIndex">The zero-based index of the page to retrieve, or null for the default page.</param>
  /// <param name="pageSize">The number of items per page, or null for the default size.</param>
  /// <param name="ascending">Whether to sort results in ascending order; if null, the default order is used.</param>
  /// <param name="orderKey">The property name to order by, or null for the default ordering.</param>
  /// <param name="partOfCharacterName">A substring to filter character names by, or null for no name filtering.</param>
  /// <param name="possibleSpecies">A comma-separated list of species to filter by, or null for no species filtering.</param>
  /// <param name="possibleClasses">A comma-separated list of classes to filter by, or null for no class filtering.</param>
  /// <param name="cancellationToken">Token to cancel the operation.</param>
  /// <returns>An HTTP result containing a collection of character names on success, or a problem response on failure.</returns>
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