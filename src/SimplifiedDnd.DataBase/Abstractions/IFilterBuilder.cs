using System.Linq.Expressions;

namespace SimplifiedDnd.DataBase.Abstractions;

public interface IFilterBuilder<T> {
  /// <summary>
/// Constructs a LINQ expression representing a filter predicate for objects of type <typeparamref name="T"/>.
/// </summary>
/// <returns>An expression that evaluates to true for objects matching the filter criteria.</returns>
Expression<Func<T, bool>> Build();
}