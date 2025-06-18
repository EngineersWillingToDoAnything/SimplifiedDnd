namespace SimplifiedDnd.WebApi.Endpoints;

internal interface IEndpoint {
  /// <summary>
/// Configures and maps the endpoint to the specified route builder.
/// </summary>
/// <param name="app">The route builder used to define endpoint routes.</param>
void MapEndpoint(IEndpointRouteBuilder app);
}