using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SimplifiedDnd.Application.Abstractions.Behaviors;
using SimplifiedDnd.Application.Abstractions.Core;

namespace SimplifiedDnd.Application;

public static class ApplicationDependencyInjection {
  /// <summary>
  /// Registers application-level services, including FluentValidation validators and MediatR handlers, from the assembly containing the <c>Result</c> type.
  /// </summary>
  /// <returns>The modified <see cref="IServiceCollection"/> instance.</returns>
  public static IServiceCollection AddApplication(this IServiceCollection services) {
    services.AddValidatorsFromAssemblyContaining<Result>(includeInternalTypes: true);

    services.AddMediatR(configuration => {
      configuration.RegisterServicesFromAssemblyContaining<Result>();
      configuration.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
    });
    return services;
  }
}