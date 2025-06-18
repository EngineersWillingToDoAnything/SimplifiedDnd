using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.DataBase.Contexts;
using SimplifiedDnd.DataBase.Repositories;
using System.Diagnostics;

namespace SimplifiedDnd.DataBase;

public static class DataBaseDependencyInjection {
  /// <summary>
  /// Configures and registers the application's database context and related repositories for dependency injection using PostgreSQL.
  /// </summary>
  /// <param name="builder">The application builder to configure.</param>
  /// <returns>The application builder with database services registered.</returns>
  public static IHostApplicationBuilder AddDataBase(this IHostApplicationBuilder builder) {
    Debug.Assert(builder is not null);

    builder.AddNpgsqlDbContext<MainDbContext>("simplifiedDndDb");

    builder.Services.AddScoped<IReadOnlyCharacterRepository, PostgreSqlCharacterRepository>();
    builder.Services.AddScoped<ICharacterRepository, PostgreSqlCharacterRepository>();

    builder.Services.AddScoped<ISpecieRepository, PostgreSqlSpecieRepository>();
    builder.Services.AddScoped<IClassRepository, PostgreSqlClassRepository>();
    builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<MainDbContext>());

    return builder;
  }
}