using Microsoft.EntityFrameworkCore;
using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.DataBase.Entities;

namespace SimplifiedDnd.DataBase.Contexts;

internal class MainDbContext(
  DbContextOptions<MainDbContext> options
) : DbContext(options), IUnitOfWork {
  internal DbSet<CharacterDbEntity> Characters { get; set; }
  internal DbSet<CharacterClassDbEntity> CharacterClasses { get; set; }
  internal DbSet<ClassDbEntity> Classes { get; set; }
  internal DbSet<SpecieDbEntity> Species { get; set; }

  /// <summary>
  /// Configures the entity models by applying all configurations from the current assembly.
  /// </summary>
  /// <param name="modelBuilder">The builder used to construct the model for the context.</param>
  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(MainDbContext).Assembly);
    base.OnModelCreating(modelBuilder);
  }

  /// <summary>
  /// Asynchronously saves all changes made in this context to the database.
  /// </summary>
  /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
  /// <returns>The number of state entries written to the database.</returns>
  public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
    int result = await base.SaveChangesAsync(cancellationToken);
    return result;
  }
}