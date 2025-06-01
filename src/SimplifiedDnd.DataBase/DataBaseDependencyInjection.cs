using Microsoft.Extensions.DependencyInjection;
using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.DataBase.Contexts;
using SimplifiedDnd.DataBase.Repositories;

namespace SimplifiedDnd.DataBase;

public static class DataBaseDependencyInjection {
  public static IServiceCollection AddDataBase(this IServiceCollection services) {
    services.AddScoped<IReadOnlyCharacterRepository, FakeCharacterRepository>();
    services.AddScoped<ICharacterRepository, FakeCharacterRepository>();
    
    services.AddScoped<ISpecieRepository, FakeSpecieRepository>();
    services.AddScoped<IClassRepository, FakeClassRepository>();
    services.AddScoped<IUnitOfWork, FakeDbContext>();
    
    return services;
  }
}