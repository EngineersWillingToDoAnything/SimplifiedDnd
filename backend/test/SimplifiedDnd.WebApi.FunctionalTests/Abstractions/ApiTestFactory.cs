using Aspire.Hosting.Testing;

namespace SimplifiedDnd.WebApi.FunctionalTests.Abstractions;

// ReSharper disable once ClassNeverInstantiated.Global
#pragma warning disable CA1515
public class ApiTestFactory() : DistributedApplicationFactory(typeof(Projects.SimplifiedDnd_AspireHost));
#pragma warning restore CA1515
