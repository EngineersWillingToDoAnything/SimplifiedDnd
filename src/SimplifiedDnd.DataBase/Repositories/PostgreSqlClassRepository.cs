using Microsoft.EntityFrameworkCore;
using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.DataBase.Contexts;
using SimplifiedDnd.DataBase.Entities;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.DataBase.Repositories;

internal sealed class PostgreSqlClassRepository(
  MainDbContext context
) : IClassRepository {
  public async Task<DndClass?> GetClassAsync(
    string name, CancellationToken cancellationToken = default
  ) {
#pragma warning disable CA1304
#pragma warning disable CA1311
#pragma warning disable CA1862
    string formattedName = name.ToUpperInvariant();

    ClassDbEntity? entity = await context.Classes.FirstOrDefaultAsync(c => 
      c.Name.ToUpper() == formattedName, cancellationToken);

    return entity?.ToDomain();
#pragma warning restore CA1304
#pragma warning restore CA1311
#pragma warning restore CA1862
  }
}