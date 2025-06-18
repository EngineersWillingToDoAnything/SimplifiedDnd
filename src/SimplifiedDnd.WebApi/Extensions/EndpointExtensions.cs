using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SimplifiedDnd.WebApi.Endpoints;

namespace SimplifiedDnd.WebApi.Extensions;

internal static class EndpointExtensions {
  /// <summary>
  /// Registers all non-abstract, non-interface types implementing <see cref="IEndpoint"/> from the specified assembly as transient services.
  /// </summary>
  /// <param name="assembly">The assembly to scan for <see cref="IEndpoint"/> implementations.</param>
  /// <returns>The updated <see cref="IServiceCollection"/> with registered endpoint services.</returns>
  internal static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly) {
    ServiceDescriptor[] serviceDescriptors = assembly
      .DefinedTypes
      .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                     type.IsAssignableTo(typeof(IEndpoint)))
      .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
      .ToArray();

    services.TryAddEnumerable(serviceDescriptors);

    return services;
  }

  /// <summary>
  /// Maps all registered <see cref="IEndpoint"/> implementations to the application's routing system using the specified route group builder or the main application.
  /// </summary>
  /// <param name="app">The web application instance.</param>
  /// <param name="routeGroupBuilder">An optional route group builder to use for endpoint mapping. If null, the main application is used.</param>
  /// <returns>The original <see cref="WebApplication"/> instance.</returns>
  internal static IApplicationBuilder MapEndpoints(
    this WebApplication app,
    RouteGroupBuilder? routeGroupBuilder = null
  ) {
    IEnumerable<IEndpoint> endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

    IEndpointRouteBuilder builder = routeGroupBuilder is null ? app : routeGroupBuilder;

    foreach (IEndpoint endpoint in endpoints) {
      endpoint.MapEndpoint(builder);
    }

    return app;
  }

  /// <summary>
  /// Adds a permission-based authorization requirement to the route handler.
  /// </summary>
  /// <param name="permission">The required permission for accessing the route.</param>
  /// <returns>The modified <see cref="RouteHandlerBuilder"/> with the authorization requirement applied.</returns>
  internal static RouteHandlerBuilder HasPermission(this RouteHandlerBuilder app, string permission) {
    return app.RequireAuthorization(permission);
  }
}