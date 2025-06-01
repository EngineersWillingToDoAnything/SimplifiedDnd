using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Response = SimplifiedDnd.WebApi.Endpoints.Characters.GetCharactersEndpoint.Response;

namespace SimplifiedDnd.WebApi.FunctionalTests.Characters;

public class GetCharactersEndpointTest(
  WebApplicationFactory<Program> factory
) : IClassFixture<WebApplicationFactory<Program>> {
  private const string Path = "/api/characters";
  private static CancellationToken TestContextToken => TestContext.Current.CancellationToken;

  [Fact(
    DisplayName = "Endpoint returns 200 OK without query parameters",
    Explicit = false)]
  public async Task EndpointReturnsOkWithoutParameters() {
    // Arrange
    HttpClient client = factory.CreateClient();

    // Act
    HttpResponseMessage response = await client.GetAsync(
      new Uri(Path, UriKind.Relative), TestContextToken);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
  }

  [Fact(
    DisplayName = "Endpoint returns a list of characters without query parameters",
    Explicit = false)]
  public async Task EndpointReturnsListWithoutParameters() {
    // Arrange
    HttpClient client = factory.CreateClient();

    // Act
    List<Response>? content = await client.GetFromJsonAsync<List<Response>>(
      new Uri(Path, UriKind.Relative), TestContextToken);

    // Assert
    content.Should().NotBeNull();
  }

  [Fact(
    DisplayName = "Returns 200 OK with infinite pagination (-1, -1)",
    Explicit = false)]
  public async Task EndpointReturnsOkWithInfinitePagination() {
    // Arrange
    var query = new Dictionary<string, string?> {
      { "page-index", "-1" },
      { "page-size", "-1" },
    };
    HttpClient client = factory.CreateClient();

    // Act
    HttpResponseMessage response = await client.GetAsync(
      new Uri(QueryHelpers.AddQueryString(Path, query), UriKind.Relative),
      TestContextToken);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
  }

  [Fact(
    DisplayName = "Returns 400 Bad Request with invalid pagination",
    Explicit = false)]
  public async Task EndpointReturnsBadRequestWithInvalidPagination() {
    // Arrange
    var query = new Dictionary<string, string?> {
      { "page-index", "-1" },
      { "page-size", "0" },
    };
    HttpClient client = factory.CreateClient();

    // Act
    HttpResponseMessage response = await client.GetAsync(
      new Uri(QueryHelpers.AddQueryString(Path, query), UriKind.Relative),
      TestContextToken);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }

  [Fact(
    DisplayName = "Returns validation error with invalid pagination",
    Explicit = false)]
  public async Task EndpointReturnsValidationErrorWithInvalidPagination() {
    // Arrange
    var query = new Dictionary<string, string?> {
      { "page-index", "-1" },
      { "page-size", "0" },
    };
    HttpClient client = factory.CreateClient();

    // Act
    HttpResponseMessage response = await client.GetAsync(
      new Uri(QueryHelpers.AddQueryString(Path, query), UriKind.Relative),
      TestContextToken);

    // Assert
    ProblemDetails? content = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContextToken);
    content.Should().NotBeNull();
    content.Title.Should().Be("Validation.General");
    content.Status.Should().Be(StatusCodes.Status400BadRequest);
    content.Detail.Should().Be("One or more validation errors occurred");
    content.Extensions.Should().NotBeNullOrEmpty();
  }
}