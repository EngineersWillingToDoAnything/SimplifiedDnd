using Microsoft.EntityFrameworkCore;
using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.DataBase.Contexts;

namespace SimplifiedDnd.DataBase.Repositories;

internal class PostgreSqlClassRepository(
  MainDbContext context
) : IClassRepository {
  public async Task<bool> CheckClassExistsAsync(
    string name, CancellationToken cancellationToken = default
  ) {
#pragma warning disable CA1304, CA1311, CA1862
    return await context.Classes.AnyAsync(c =>
      c.Name.ToUpper() == name.ToUpper(), cancellationToken);
#pragma warning restore CA1304, CA1311, CA1862
  }
}