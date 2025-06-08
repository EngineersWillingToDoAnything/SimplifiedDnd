using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using SimplifiedDnd.WebApi.FunctionalTests.Abstractions;
using System.Net;
using System.Net.Http.Json;
using Response = SimplifiedDnd.WebApi.Endpoints.Characters.GetCharactersEndpoint.Response;

namespace SimplifiedDnd.WebApi.FunctionalTests.Characters;

public class GetCharactersEndpointTest(
  ApiTestFactory factory
) : IClassFixture<ApiTestFactory>,
    IAsyncLifetime {
  private const string Path = "/api/characters";
  private static CancellationToken TestContextToken => TestContext.Current.CancellationToken;
  
#pragma warning disable CA1816
  public ValueTask DisposeAsync() {
    return ValueTask.CompletedTask;
  }
#pragma warning restore CA1816

  public async ValueTask InitializeAsync() {
    await factory.StartAsync(TestContextToken);
  }

  [Fact(
    DisplayName = "Returns 200 OK without query parameters",
    Explicit = false)]
  public async Task EndpointReturnsOkWithoutParameters() {
    // Arrange
    HttpClient client = factory.CreateHttpClient("api");

    // Act
    HttpResponseMessage response = await client.GetAsync(
      new Uri(Path, UriKind.Relative), TestContextToken);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
  }

  [Fact(
    DisplayName = "Returns a list of characters without query parameters",
    Explicit = false)]
  public async Task EndpointReturnsListWithoutParameters() {
    // Arrange
    HttpClient client = factory.CreateHttpClient("api");

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
    HttpClient client = factory.CreateHttpClient("api");

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
    HttpClient client = factory.CreateHttpClient("api");

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
    HttpClient client = factory.CreateHttpClient("api");

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

  [Theory(
    DisplayName = "Returns 200 OK with valid order parameter",
    Explicit = false)]
  [InlineData("id")]
  [InlineData("name")]
  [InlineData("mainClass")]
  [InlineData("specie")]
  public async Task EndpointReturnsOkWithValidOrderParameter(string orderKey) {
    var query = new Dictionary<string, string?> {
      { "order-asc", "true" },
      { "order-key", orderKey },
    };
    HttpClient client = factory.CreateHttpClient("api");

    // Act
    HttpResponseMessage response = await client.GetAsync(
      new Uri(QueryHelpers.AddQueryString(Path, query), UriKind.Relative),
      TestContextToken);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
  }

  [Fact(
    DisplayName = "Returns validation error with invalid order",
    Explicit = false)]
  public async Task EndpointReturnsValidationErrorWithInvalidOrder() {
    // Arrange
    var query = new Dictionary<string, string?> {
      { "order-asc", "true" },
      { "order-key", string.Empty },
    };
    HttpClient client = factory.CreateHttpClient("api");

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

  [Theory(
    DisplayName = "Returns a list of characters with all query parameters",
    Explicit = false)]
  [InlineData("id")]
  [InlineData("name")]
  [InlineData("mainClass")]
  [InlineData("specie")]
  public async Task EndpointReturnsListWithAllParameters(string orderKey) {
    // Arrange
    var query = new Dictionary<string, string?> {
      { "page-index", "0" },
      { "page-size", "10" },
      { "order-asc", "false" },
      { "order-key", orderKey },
    };
    HttpClient client = factory.CreateHttpClient("api");

    // Act
    List<Response>? content = await client.GetFromJsonAsync<List<Response>>(
      new Uri(QueryHelpers.AddQueryString(Path, query), UriKind.Relative),
      TestContextToken);

    // Assert
    content.Should().NotBeNull();
  }
}