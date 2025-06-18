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

  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(MainDbContext).Assembly);
    base.OnModelCreating(modelBuilder);
  }

  public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
    int result = await base.SaveChangesAsync(cancellationToken);
    return result;
  }
}