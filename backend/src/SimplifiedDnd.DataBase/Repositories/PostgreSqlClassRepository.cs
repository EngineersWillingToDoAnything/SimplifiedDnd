using Microsoft.EntityFrameworkCore;
using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.DataBase.Contexts;
using SimplifiedDnd.DataBase.Entities;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.DataBase.Repositories;

internal class PostgreSqlClassRepository(
  MainDbContext context
) : IClassRepository {
  public async Task<bool> CheckClassExistsAsync(
    string name, CancellationToken cancellationToken = default
  ) {
    string formattedName = name.ToUpperInvariant();

#pragma warning disable CA1304, CA1311, CA1862
    return await context.Classes.AnyAsync(c => 
      c.Name.ToUpper() == formattedName, cancellationToken);
#pragma warning restore CA1304, CA1311, CA1862
  }
}