using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.Application.Abstractions.Characters;

public interface ISpecieRepository {
  Task<Specie?> GetSpecieAsync(string name, CancellationToken cancellationToken);
}