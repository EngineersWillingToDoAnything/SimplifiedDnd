using Microsoft.EntityFrameworkCore;
using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.DataBase.Contexts;
using SimplifiedDnd.DataBase.Entities;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.DataBase.Repositories;

internal sealed class PostgreSqlSpecieRepository(
  MainDbContext context
) : ISpecieRepository {
  public async Task<Specie?> GetSpecieAsync(
    string name, CancellationToken cancellationToken
  ) {
    SpecieDbEntity? entity = await context.Species.FirstOrDefaultAsync(s =>
      s.Name == name, cancellationToken);

    return entity?.ToDomain();
  }
}