using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SimplifiedDnd.Database.IntegrationTests.Abstractions;

internal interface IInfrastructureService : IAsyncLifetime {
  void AddHostConfiguration(IConfigurationBuilder config);
  void Configure(IServiceCollection services);
}