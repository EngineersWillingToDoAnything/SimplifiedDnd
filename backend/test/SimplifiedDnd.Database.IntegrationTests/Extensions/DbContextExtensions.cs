using Microsoft.EntityFrameworkCore;

namespace SimplifiedDnd.Database.IntegrationTests.Extensions;

internal static class DbContextExtensions {
  private static string? _oldConnectionString;

  public static void SetInvalidConnectionString(this DbContext context) {
    _oldConnectionString = context.Database.GetConnectionString();
    context.Database.SetConnectionString(
      "Host=localhost;Port=56791;Username=INVALID;Password=INVALID;Database=mainDb;CancellationTimeout=1;CommandTimeout=1;Timeout=1");
  }

  public static void RestoreConnectionString(this DbContext context) {
    context.Database.SetConnectionString(_oldConnectionString);
  }
}