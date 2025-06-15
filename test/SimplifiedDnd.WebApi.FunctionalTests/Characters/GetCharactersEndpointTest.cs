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
) : BaseEndpointTest(factory) {
  private const string Path = "/api/characters";
  private static CancellationToken TestContextToken => TestContext.Current.CancellationToken;

  private readonly ApiTestFactory _factory = factory;

  [Fact(
    DisplayName = "Returns 200 OK without query parameters",
    Explicit = false)]
  public async Task EndpointReturnsOkWithoutParameters() {
    // Arrange
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);

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
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);

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
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);

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
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);

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
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);

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
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);

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
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);

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

  [Fact(
    DisplayName = "Returns 200 OK with part of the character name to filter",
    Explicit = false)]
  public async Task EndpointReturnsOkWithPartOfTheCharacterNameToFilter() {
    var query = new Dictionary<string, string?> {
      { "filter-name", "bruce" }
    };
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);

    // Act
    HttpResponseMessage response = await client.GetAsync(
      new Uri(QueryHelpers.AddQueryString(Path, query), UriKind.Relative),
      TestContextToken);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
  }

  [Fact(
    DisplayName = "Returns 200 OK with the name of the specie to filter",
    Explicit = false)]
  public async Task EndpointReturnsOkWithTheNameOfTheSpecieToFilter() {
    var query = new Dictionary<string, string?> {
      { "filter-species", "dwarf" },
    };
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);

    // Act
    HttpResponseMessage response = await client.GetAsync(
      new Uri(QueryHelpers.AddQueryString(Path, query), UriKind.Relative),
      TestContextToken);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
  }

  [Fact(
    DisplayName = "Returns 200 OK with more than one specie to filter",
    Explicit = false)]
  public async Task EndpointReturnsOkWithMoreThanOneSpecieToFilter() {
    var query = new Dictionary<string, string?> {
      { "filter-species", "human,tiefling,elf" },
    };
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);

    // Act
    HttpResponseMessage response = await client.GetAsync(
      new Uri(QueryHelpers.AddQueryString(Path, query), UriKind.Relative),
      TestContextToken);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
  }

  [Fact(
    DisplayName = "Returns 200 OK with the name of the class to filter",
    Explicit = false)]
  public async Task EndpointReturnsOkWithTheNameOfTheClassToFilter() {
    var query = new Dictionary<string, string?> {
      { "filter-classes", "fighter" },
    };
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);

    // Act
    HttpResponseMessage response = await client.GetAsync(
      new Uri(QueryHelpers.AddQueryString(Path, query), UriKind.Relative),
      TestContextToken);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
  }

  [Fact(
    DisplayName = "Returns 200 OK with more than one class to filter",
    Explicit = false)]
  public async Task EndpointReturnsOkWithMoreThanOneClassToFilter() {
    var query = new Dictionary<string, string?> {
      { "filter-classes", "paladin,monk,ranger" },
    };
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);

    // Act
    HttpResponseMessage response = await client.GetAsync(
      new Uri(QueryHelpers.AddQueryString(Path, query), UriKind.Relative),
      TestContextToken);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
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
      { "filter-name", "bruce" },
      { "filter-species", "human,tiefling" },
      { "filter-classes", "barbarian,druid,cleric" },
    };
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);

    // Act
    List<Response>? content = await client.GetFromJsonAsync<List<Response>>(
      new Uri(QueryHelpers.AddQueryString(Path, query), UriKind.Relative),
      TestContextToken);

    // Assert
    content.Should().NotBeNull();
  }
}