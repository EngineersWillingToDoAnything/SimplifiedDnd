using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.DataBase.Contexts;
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
    builder.Services.AddScoped<IClassRepository, PostgreSqlClassRepository>();
    builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<MainDbContext>());

    return builder;
  }
}