namespace SimplifiedDnd.WebApi.FunctionalTests.Abstractions;

#pragma warning disable CA1515
public abstract class BaseEndpointTest(
#pragma warning restore CA1515
  ApiTestFactory factory
) : IClassFixture<ApiTestFactory>,
  IAsyncLifetime {
  protected const string ApiResourceName = "api";

#pragma warning disable CA1816
  public ValueTask DisposeAsync() => ValueTask.CompletedTask;
#pragma warning restore CA1816

  public async ValueTask InitializeAsync() {
    await factory.StartAsync(TestContext.Current.CancellationToken);
  }
}