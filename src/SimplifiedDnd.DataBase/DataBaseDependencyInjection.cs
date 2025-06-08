using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.DataBase.Contexts;
using SimplifiedDnd.DataBase.Entities;
using SimplifiedDnd.DataBase.Repositories;
using System.Diagnostics;

namespace SimplifiedDnd.DataBase;

public static class DataBaseDependencyInjection {
  public static IHostApplicationBuilder AddDataBase(this IHostApplicationBuilder builder) {
    Debug.Assert(builder is not null);

    builder.AddNpgsqlDbContext<MainDbContext>("simplifiedDndDb");

    builder.Services.AddScoped<IReadOnlyCharacterRepository, PostgreSqlCharacterRepository>();
    builder.Services.AddScoped<ICharacterRepository, PostgreSqlCharacterRepository>();

    builder.Services.AddScoped<ISpecieRepository, PostgreSqlSpecieRepository>();
    builder.Services.AddScoped<IClassRepository, FakeClassRepository>();
    builder.Services.AddScoped<IUnitOfWork, MainDbContext>();

    return builder;
  }
}

public static class HostExtensions {
  public static void CreateDbIfNotExists(this IHost host) {
    Debug.Assert(host is not null);
    
    using IServiceScope scope = host.Services.CreateScope();

    MainDbContext context = scope.ServiceProvider.GetRequiredService<MainDbContext>();
    try {
      context.Database.EnsureCreated();
      context.Add(new SpecieDbEntity { Name = "Human", Speed = 30 });
      context.SaveChanges();
    } catch (NpgsqlException) { // Testing
    }
  }
}