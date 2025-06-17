using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.Application.Abstractions.Characters;

public interface IClassRepository {
  Task<bool> CheckClassExistsAsync(
    string name, CancellationToken cancellationToken = default);
}