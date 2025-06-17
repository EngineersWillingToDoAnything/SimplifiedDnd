using FluentAssertions;
using SimplifiedDnd.WebApi.FunctionalTests.Abstractions;
using System.Net;
using System.Net.Mime;
using System.Text;

namespace SimplifiedDnd.WebApi.FunctionalTests.Characters;

public class CreateCharacterEndpointTest(
  ApiTestFactory factory
) : BaseEndpointTest(factory) {
  private const string Path = "/api/character";
  private static CancellationToken TestContextToken => TestContext.Current.CancellationToken;

  private readonly ApiTestFactory _factory = factory;

  [Fact(
    DisplayName = "Returns 400 Bad Request with empty body",
    Explicit = false)]
  public async Task EndpointReturnsBadRequestWithEmptyBody() {
    // Arrange
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);
    using var content = new StringContent(
      "{}", Encoding.UTF8, MediaTypeNames.Application.Json);

    // Act
    HttpResponseMessage response = await client.PostAsync(
      new Uri(Path, UriKind.Relative), content, TestContextToken);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }

  private class WhiteSpaceData() : TheoryData<string>(string.Empty, " ");

  [Theory(
    DisplayName = "Returns 400 Bad Request with empty character name",
    Explicit = false)]
  [ClassData(typeof(WhiteSpaceData))]
  public async Task EndpointReturnsBadRequestWithEmptyCharacterName(string playerName) {
    // Arrange
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);
    string requestBody =
      $$"""
        {
          "name": "{{playerName}}",
          "player_name": "-",
          "specie_name": "-",
          "classes": [
            {
              "name": "-",
              "level": 1
            }
          ]
        }
        """;
    using var content = new StringContent(
      requestBody, Encoding.UTF8, MediaTypeNames.Application.Json);

    //  Act
    HttpResponseMessage response = await client.PostAsync(
      new Uri(Path, UriKind.Relative), content, TestContextToken);

    // Arrange
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }

  [Theory(
    DisplayName = "Returns 400 Bad Request with empty player name",
    Explicit = false)]
  [ClassData(typeof(WhiteSpaceData))]
  public async Task EndpointReturnsBadRequestWithEmptyPlayerName(string playerName) {
    // Arrange
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);
    string requestBody =
      $$"""
        {
          "name": "-",
          "player_name": "{{playerName}}",
          "specie_name": "-",
          "classes": [
            {
              "name": "-",
              "level": 1
            }
          ]
        }
        """;
    using var content = new StringContent(
      requestBody, Encoding.UTF8, MediaTypeNames.Application.Json);

    //  Act
    HttpResponseMessage response = await client.PostAsync(
      new Uri(Path, UriKind.Relative), content, TestContextToken);

    // Arrange
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }

  [Theory(
    DisplayName = "Returns 400 Bad Request with empty specie name",
    Explicit = false)]
  [ClassData(typeof(WhiteSpaceData))]
  public async Task EndpointReturnsBadRequestWithEmptySpecieName(string specieName) {
    // Arrange
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);
    string requestBody =
      $$"""
        {
          "name": "-",
          "player_name": "-",
          "specie_name": "{{specieName}}"
        }
        """;
    using var content = new StringContent(
      requestBody, Encoding.UTF8, MediaTypeNames.Application.Json);

    //  Act
    HttpResponseMessage response = await client.PostAsync(
      new Uri(Path, UriKind.Relative), content, TestContextToken);

    // Arrange
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }

  [Fact(
    DisplayName = "Returns 404 Not Found with invalid specie",
    Explicit = false)]
  public async Task EndpointReturnsNotFoundWithInvalidSpecie() {
    // Arrange
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);
    const string requestBody =
      """
      {
        "name": "-",
        "player_name": "-",
        "specie_name": "-",
        "classes": [
          {
            "name": "-",
            "level": 1
          }
        ]
      }
      """;
    using var content = new StringContent(
      requestBody, Encoding.UTF8, MediaTypeNames.Application.Json);

    //  Act
    HttpResponseMessage response = await client.PostAsync(
      new Uri(Path, UriKind.Relative), content, TestContextToken);

    // Arrange
    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }

  [Fact(
    DisplayName = "Returns 404 Not Found with invalid class name",
    Explicit = false)]
  public async Task EndpointReturnsNotFoundWithInvalidClass() {
    // Arrange
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);
    const string requestBody =
      """
      {
        "name": "-",
        "player_name": "-",
        "specie_name": "Dragonborn",
        "classes": [
          {
            "name": "-",
            "level": 1
          }
        ]
      }
      """;
    using var content = new StringContent(
      requestBody, Encoding.UTF8, MediaTypeNames.Application.Json);

    //  Act
    HttpResponseMessage response = await client.PostAsync(
      new Uri(Path, UriKind.Relative), content, TestContextToken);

    // Arrange
    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }

  [Fact(
    DisplayName = "Returns 404 Not Found with one valid class and at least one invalid class",
    Explicit = false)]
  public async Task EndpointReturnsNotFoundWithAtLeastOneInvalidClass() {
    // Arrange
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);
    const string requestBody =
      """
      {
        "name": "-",
        "player_name": "-",
        "specie_name": "Dragonborn",
        "classes": [
          {
            "name": "Artificer",
            "level": 1
          },
          {
            "name": "-",
            "level": 1
          }
        ]
      }
      """;
    using var content = new StringContent(
      requestBody, Encoding.UTF8, MediaTypeNames.Application.Json);

    //  Act
    HttpResponseMessage response = await client.PostAsync(
      new Uri(Path, UriKind.Relative), content, TestContextToken);

    // Arrange
    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }

  [Fact(
    DisplayName = "Returns 201 Created with valid request",
    Explicit = false)]
  public async Task EndpointReturnsCreatedWithValidRequest() {
    // Arrange
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);
    string requestBody =
      $$"""
        {
          "name": "{{Guid.CreateVersion7()}}",
          "player_name": "-",
          "specie_name": "human",
          "classes": [
            {
              "name": "barbarian",
              "level": 1
            }
          ]
        }
        """;
    using var content = new StringContent(
      requestBody, Encoding.UTF8, MediaTypeNames.Application.Json);

    //  Act
    HttpResponseMessage response = await client.PostAsync(
      new Uri(Path, UriKind.Relative), content, TestContextToken);

    // Arrange
    response.StatusCode.Should().Be(HttpStatusCode.Created);
  }

  [Fact(
    DisplayName = "Returns character GUID with valid request",
    Explicit = false)]
  public async Task EndpointReturnsGuidWithValidRequest() {
    // Arrange
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);
    string requestBody =
      $$"""
        {
          "name": "{{Guid.CreateVersion7()}}",
          "player_name": "-",
          "specie_name": "human",
          "classes": [
            {
              "name": "barbarian",
              "level": 1
            }
          ]
        }
        """;
    using var content = new StringContent(
      requestBody, Encoding.UTF8, MediaTypeNames.Application.Json);

    //  Act
    HttpResponseMessage response = await client.PostAsync(
      new Uri(Path, UriKind.Relative), content, TestContextToken);

    // Arrange
    string result = await response.Content.ReadAsStringAsync(TestContextToken);
    Guid.TryParse(result.Trim('"'), out _).Should().BeTrue();
  }
}