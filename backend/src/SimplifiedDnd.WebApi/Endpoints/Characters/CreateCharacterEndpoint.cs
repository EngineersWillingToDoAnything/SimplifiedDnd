using Microsoft.AspNetCore.Mvc;
using MediatR;
using SimplifiedDnd.Application.Abstractions.Core;
using SimplifiedDnd.Application.Characters.CreateCharacter;
using SimplifiedDnd.Domain.Characters;
using SimplifiedDnd.WebApi.Abstractions;
using SimplifiedDnd.WebApi.Extensions;
using System.Text.Json.Serialization;

namespace SimplifiedDnd.WebApi.Endpoints.Characters;

// ReSharper disable once UnusedType.Global
internal class CreateCharacterEndpoint : IEndpoint {
  // ReSharper disable once ClassNeverInstantiated.Global
  internal class ClassRequest {
    [JsonPropertyName("name")] public string ClassName { get; init; } = null!;
    [JsonPropertyName("level")] public int Level { get; init; }
  }

  // ReSharper disable once MemberCanBePrivate.Global
  internal class Request {
    // ReSharper disable once MemberCanBePrivate.Global
    [JsonPropertyName("name")] public string Name { get; init; } = null!;

    // ReSharper disable once MemberCanBePrivate.Global
    [JsonPropertyName("player_name")] public string PlayerName { get; init; } = null!;

    // ReSharper disable once MemberCanBePrivate.Global
    [JsonPropertyName("specie_name")] public string SpecieName { get; init; } = null!;

    // ReSharper disable once MemberCanBePrivate.Global
    [JsonPropertyName("classes")] public IReadOnlyCollection<ClassRequest> Classes { get; init; } = null!;

    internal CreateCharacterCommand ToCommand() {
      return new CreateCharacterCommand {
        Name = Name,
        PlayerName = PlayerName,
        SpecieName = SpecieName,
        Classes = [
          ..(Classes ?? []).Select(c => new DndClass {
            Name = c.ClassName,
            Level = new Level(c.Level),
          })
        ]
      };
    }
  }

  public void MapEndpoint(IEndpointRouteBuilder app) {
    app.MapPost("api/character", Handle)
      .Produces<Guid>(StatusCodes.Status201Created)
      .Produces(StatusCodes.Status404NotFound)
      .Produces(StatusCodes.Status409Conflict)
      .WithTags(Tags.Characters);
  }

  private static async Task<IResult> Handle(
    [FromBody] Request request,
    ISender sender,
    CancellationToken cancellationToken
  ) {
    CreateCharacterCommand command = request.ToCommand();

    Result<Guid> result = await sender.Send(command, cancellationToken);

    return result.Match(
      guid => Results.Created(new Uri($"/api/character/{guid}", UriKind.Relative), guid),
      CustomResults.Problem);
  }
}