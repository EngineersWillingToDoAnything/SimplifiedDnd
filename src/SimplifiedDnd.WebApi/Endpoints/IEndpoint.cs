namespace SimplifiedDnd.WebApi.Endpoints;

internal interface IEndpoint {
  void MapEndpoint(IEndpointRouteBuilder app);
}