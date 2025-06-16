using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SimplifiedDnd.WebApi.Extensions;
using System.Text.Json.Serialization;

namespace SimplifiedDnd.WebApi.Endpoints.Characters;

internal class CreateCharacterEndpoint : IEndpoint {
  internal class Request {
    [JsonPropertyName("name")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    public string Name { get; init; } = null!;

    [JsonPropertyName("specie_name")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    public string SpecieName { get; init; } = null!;
  }

  internal class RequestValidator : AbstractValidator<Request> {
    public RequestValidator() {
      RuleFor(r => r.Name).NotEmpty();

      RuleFor(r => r.SpecieName).NotEmpty();
    }
  }

  public void MapEndpoint(IEndpointRouteBuilder app) {
    app.MapPost("api/character", Handle)
      .WithRequestValidation<Request>()
      .Produces<Guid>(StatusCodes.Status201Created)
      .WithTags(Tags.Characters);
  }

  private static IResult Handle(
    [FromBody] Request request
  ) {
    return request.Name.Length == 0 || request.SpecieName.Length == 0
      ? Results.BadRequest()
      : Results.Created();
  }
}