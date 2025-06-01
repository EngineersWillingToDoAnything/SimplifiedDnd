using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.Application.Abstractions.Characters;

public interface IClassRepository {
  Task<DndClass?> GetClassAsync(
    string name, CancellationToken cancellationToken = default);
}