using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OpenTelemetry.Trace;
using SimplifiedDnd.DataBase.Contexts;
using SimplifiedDnd.DataBase.Entities;
using System.Diagnostics;

namespace SimplifiedDnd.MigrationService;

internal class Worker(
  IServiceProvider serviceProvider,
  IHostApplicationLifetime hostApplicationLifetime
) : BackgroundService {
  internal const string ActivitySourceName = "Migrations";
  private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

  protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
    using Activity? activity = ActivitySource.StartActivity(ActivityKind.Client);

    try {
      using IServiceScope scope = serviceProvider.CreateScope();
      MainDbContext dbContext = scope.ServiceProvider.GetRequiredService<MainDbContext>();

      await RunMigrationAsync(dbContext, stoppingToken);
      await SeedDataAsync(dbContext, stoppingToken);
    } catch (Exception ex) {
      activity?.RecordException(ex);
      throw;
    }

    hostApplicationLifetime.StopApplication();
  }

  private static async Task RunMigrationAsync(
    MainDbContext dbContext, CancellationToken stoppingToken
  ) {
    IExecutionStrategy strategy = dbContext.Database.CreateExecutionStrategy();
    await strategy.ExecuteAsync(async () =>
      await dbContext.Database.MigrateAsync(stoppingToken));
  }

  private static async Task SeedDataAsync(
    MainDbContext dbContext, CancellationToken stoppingToken
  ) {
    IExecutionStrategy strategy = dbContext.Database.CreateExecutionStrategy();
    await strategy.ExecuteAsync(async () => {
      await using IDbContextTransaction transaction =
        await dbContext.Database.BeginTransactionAsync(stoppingToken);

      dbContext.Species.AddRange(
        new SpecieDbEntity { Name = "Dragonborn", Speed = 30 },
        new SpecieDbEntity { Name = "Dwarf", Speed = 25 },
        new SpecieDbEntity { Name = "Human", Speed = 30 }
      );
      dbContext.Classes.AddRange(
        new ClassDbEntity { Name = "Artificer" },
        new ClassDbEntity { Name = "Barbarian" },
        new ClassDbEntity { Name = "Bard" }
      );

      await dbContext.SaveChangesAsync(stoppingToken);
      await transaction.CommitAsync(stoppingToken);
    });
  }
}