using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SimplifiedDnd.WebApi.FunctionalTests.Abstractions;
using SimplifiedDnd.WebApi.FunctionalTests.Extensions;
using System.Net;
using System.Net.Http.Json;
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
    Explicit = true)]
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

  [Fact(
    DisplayName = "Returns validation error with empty body",
    Explicit = true)]
  public async Task EndpointReturnsValidationErrorWithEmptyBody() {
    // Arrange
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);
    using var content = new StringContent(
      "{}", Encoding.UTF8, MediaTypeNames.Application.Json);

    // Act
    HttpResponseMessage response = await client.PostAsync(
      new Uri(Path, UriKind.Relative), content, TestContextToken);

    // Assert
    ProblemDetails? problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContextToken);
    problemDetails.Should().BeValidationError();
  }

  private class WhiteSpaceData() : TheoryData<string>(string.Empty, " ");

  [Theory(
    DisplayName = "Returns 400 Bad Request with empty character name",
    Explicit = true)]
  [ClassData(typeof(WhiteSpaceData))]
  public async Task EndpointReturnsBadRequestWithEmptyCharacterName(string characterName) {
    // Arrange
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);
    string requestBody =
      $$"""
        {
          "name": "{{characterName}}",
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
    DisplayName = "Returns validation error with empty character name",
    Explicit = true)]
  [ClassData(typeof(WhiteSpaceData))]
  public async Task EndpointReturnsValidationErrorWithEmptyCharacterName(string characterName) {
    // Arrange
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);
    string requestBody =
      $$"""
        {
          "name": "{{characterName}}",
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

    // Act
    HttpResponseMessage response = await client.PostAsync(
      new Uri(Path, UriKind.Relative), content, TestContextToken);

    // Assert
    ProblemDetails? problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContextToken);
    problemDetails.Should().BeValidationError();
  }

  [Theory(
    DisplayName = "Returns 400 Bad Request with empty player name",
    Explicit = true)]
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
    DisplayName = "Returns validation error with empty player name",
    Explicit = true)]
  [ClassData(typeof(WhiteSpaceData))]
  public async Task EndpointReturnsValidationErrorWithEmptyPlayerName(string playerName) {
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

    // Act
    HttpResponseMessage response = await client.PostAsync(
      new Uri(Path, UriKind.Relative), content, TestContextToken);

    // Assert
    ProblemDetails? problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContextToken);
    problemDetails.Should().BeValidationError();
  }

  [Theory(
    DisplayName = "Returns 400 Bad Request with empty specie name",
    Explicit = true)]
  [ClassData(typeof(WhiteSpaceData))]
  public async Task EndpointReturnsBadRequestWithEmptySpecieName(string specieName) {
    // Arrange
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);
    string requestBody =
      $$"""
        {
          "name": "-",
          "player_name": "-",
          "specie_name": "{{specieName}}",
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
    DisplayName = "Returns validation error with empty specie name",
    Explicit = true)]
  [ClassData(typeof(WhiteSpaceData))]
  public async Task EndpointReturnsValidationErrorWithEmptySpecieName(string specieName) {
    // Arrange
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);
    string requestBody =
      $$"""
        {
          "name": "-",
          "player_name": "-",
          "specie_name": "{{specieName}}",
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

    // Act
    HttpResponseMessage response = await client.PostAsync(
      new Uri(Path, UriKind.Relative), content, TestContextToken);

    // Assert
    ProblemDetails? problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContextToken);
    problemDetails.Should().BeValidationError();
  }

  [Theory(
    DisplayName = "Returns 400 Bad Request with empty class name",
    Explicit = true)]
  [ClassData(typeof(WhiteSpaceData))]
  public async Task EndpointReturnsBadRequestWithEmptyClassName(string className) {
    // Arrange
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);
    string requestBody =
      $$"""
        {
          "name": "-",
          "player_name": "-",
          "specie_name": "-",
          "classes": [
            {
              "name": "{{className}}",
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
    DisplayName = "Returns validation error with empty class name",
    Explicit = true)]
  [ClassData(typeof(WhiteSpaceData))]
  public async Task EndpointReturnsValidationErrorWithEmptyClassName(string className) {
    // Arrange
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);
    string requestBody =
      $$"""
        {
          "name": "-",
          "player_name": "-",
          "specie_name": "-",
          "classes": [
            {
              "name": "{{className}}",
              "level": 1
            }
          ]
        }
        """;
    using var content = new StringContent(
      requestBody, Encoding.UTF8, MediaTypeNames.Application.Json);

    // Act
    HttpResponseMessage response = await client.PostAsync(
      new Uri(Path, UriKind.Relative), content, TestContextToken);

    // Assert
    ProblemDetails? problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContextToken);
    problemDetails.Should().BeValidationError();
  }

  [Fact(
    DisplayName = "Returns 400 Bad Request with class level below minimum",
    Explicit = true)]
  public async Task EndpointReturnsBadRequestWithClassLevelBelowMinimum() {
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
            "level": 0
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

  [Fact(
    DisplayName = "Returns validation error with class level below minimum",
    Explicit = true)]
  public async Task EndpointReturnsValidationErrorWithClassLevelBelowMinimum() {
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
            "level": 0
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
    ProblemDetails? problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContextToken);
    problemDetails.Should().BeValidationError();
  }

  [Fact(
    DisplayName = "Returns 400 Bad Request with class level above maximum",
    Explicit = true)]
  public async Task EndpointReturnsBadRequestWithClassLevelAboveMaximum() {
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
            "level": 21
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

  [Fact(
    DisplayName = "Returns validation error with class level above maximum",
    Explicit = true)]
  public async Task EndpointReturnsValidationErrorWithClassLevelAboveMinimum() {
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
            "level": 21
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
    ProblemDetails? problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContextToken);
    problemDetails.Should().BeValidationError();
  }

  [Fact(
    DisplayName = "Returns 400 Bad Request with repeated class name",
    Explicit = true)]
  public async Task EndpointReturnsBadRequestWithRepeatedClassName() {
    // Arrange
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);
    const string className = "-";
    const string requestBody =
      $$"""
        {
          "name": "-",
          "player_name": "-",
          "specie_name": "-",
          "classes": [
            {
              "name": "{{className}}",
              "level": 1
            },
            {
              "name": "{{className}}",
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

  [Fact(
    DisplayName = "Returns validation error with repeated class name",
    Explicit = true)]
  public async Task EndpointReturnsValidationErrorWithRepeatedClassName() {
    // Arrange
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);
    const string className = "-";
    const string requestBody =
      $$"""
        {
          "name": "-",
          "player_name": "-",
          "specie_name": "-",
          "classes": [
            {
              "name": "{{className}}",
              "level": 1
            },
            {
              "name": "{{className}}",
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
    ProblemDetails? problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContextToken);
    problemDetails.Should().BeValidationError();
  }

  [Fact(
    DisplayName = "Returns 404 Not Found with non existing specie",
    Explicit = true)]
  public async Task EndpointReturnsNotFoundWithNonExistingSpecie() {
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
    DisplayName = "Returns 404 Not Found with non existing class name",
    Explicit = true)]
  public async Task EndpointReturnsNotFoundWithNonExistingClass() {
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
    DisplayName = "Returns 404 Not Found with at least one non existing class",
    Explicit = true)]
  public async Task EndpointReturnsNotFoundWithAtLeastOneNonExistingClass() {
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
    DisplayName = "Returns 409 Conflict if character already exists",
    Explicit = true)]
  public async Task EndpointReturnsConflictWithExistingCharacter() {
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
    await client.PostAsync(new Uri(Path, UriKind.Relative), content, TestContextToken); // ensure character exists

    //  Act
    HttpResponseMessage response = await client.PostAsync(
      new Uri(Path, UriKind.Relative), content, TestContextToken);

    // Arrange
    response.StatusCode.Should().Be(HttpStatusCode.Conflict);
  }

  [Fact(
    DisplayName = "Returns 201 Created with valid request",
    Explicit = true)]
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
    Explicit = true)]
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