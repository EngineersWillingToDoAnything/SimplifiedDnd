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
    const string requestBody = "{}";
    using var content = new StringContent(
      requestBody, Encoding.UTF8, MediaTypeNames.Application.Json);

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
  public async Task EndpointReturnsBadRequestWithEmptyCharacterName(string characterName) {
    // Arrange
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);
    string requestBody =
      $$"""
        {
          "name": "{{characterName}}",
          "specie_name": "-"
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
    DisplayName = "Returns 201 Created with valid request",
    Explicit = false)]
  public async Task EndpointReturnsCreatedWithValidRequest() {
    // Arrange
    HttpClient client = _factory.CreateHttpClient(ApiResourceName);
    const string requestBody =
      """
      {
        "name": "-",
        "specie_name": "-"
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
}