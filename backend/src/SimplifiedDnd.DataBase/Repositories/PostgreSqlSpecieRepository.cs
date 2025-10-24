using Microsoft.EntityFrameworkCore;
using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.DataBase.Contexts;
using SimplifiedDnd.DataBase.Entities;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.DataBase.Repositories;

internal class PostgreSqlSpecieRepository(
  MainDbContext context
) : ISpecieRepository {
  public async Task<Specie?> GetSpecieAsync(
    string name, CancellationToken cancellationToken
  ) {
    string formattedName = name.ToUpperInvariant();
    
#pragma warning disable CA1304, CA1311, CA1862
    SpecieDbEntity? entity = await context.Species.FirstOrDefaultAsync(s =>
      s.Name.ToUpper() == formattedName, cancellationToken);
#pragma warning restore CA1862, CA1311, CA1304
    
    return entity?.ToDomain();
  }
}