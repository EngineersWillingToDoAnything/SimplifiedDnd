using Microsoft.EntityFrameworkCore;
using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.DataBase.Contexts;
using SimplifiedDnd.DataBase.Entities;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.DataBase.Repositories;

internal sealed class PostgreSqlClassRepository(
  MainDbContext context
) : IClassRepository {
  /// <summary>
  /// Asynchronously determines whether a class with the specified name exists in the database, using a case-insensitive comparison.
  /// </summary>
  /// <param name="name">The name of the class to check for existence.</param>
  /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
  /// <returns>True if a class with the specified name exists; otherwise, false.</returns>
  public async Task<bool> CheckClassExistsAsync(
    string name, CancellationToken cancellationToken = default
  ) {
#pragma warning disable CA1304, CA1311, CA1862
    string formattedName = name.ToUpperInvariant();

    return await context.Classes.AnyAsync(c => 
      c.Name.ToUpper() == formattedName, cancellationToken);
#pragma warning restore CA1304, CA1311, CA1862
  }
}