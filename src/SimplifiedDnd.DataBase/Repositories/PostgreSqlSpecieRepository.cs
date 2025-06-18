using Microsoft.EntityFrameworkCore;
using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.DataBase.Contexts;
using SimplifiedDnd.DataBase.Entities;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.DataBase.Repositories;

internal sealed class PostgreSqlSpecieRepository(
  MainDbContext context
) : ISpecieRepository {
  /// <summary>
  /// Asynchronously retrieves a species by name, performing a case-insensitive search.
  /// </summary>
  /// <param name="name">The name of the species to retrieve.</param>
  /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
  /// <returns>The matching <see cref="Specie"/> if found; otherwise, null.</returns>
  public async Task<Specie?> GetSpecieAsync(
    string name, CancellationToken cancellationToken
  ) {
#pragma warning disable CA1304
#pragma warning disable CA1311
#pragma warning disable CA1862
    string formattedName = name.ToUpperInvariant();
    
    SpecieDbEntity? entity = await context.Species.FirstOrDefaultAsync(s =>
      s.Name.ToUpper() == formattedName, cancellationToken);
    
    return entity?.ToDomain();
#pragma warning restore CA1862
#pragma warning restore CA1311
#pragma warning restore CA1304
  }
}