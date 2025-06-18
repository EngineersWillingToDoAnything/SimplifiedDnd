namespace SimplifiedDnd.DataBase.Abstractions;

public interface IOrderBuilder<T> {
  /// <summary>
/// Applies ordering to the provided queryable sequence and returns an ordered queryable.
/// </summary>
/// <param name="queryable">The source queryable sequence to be ordered.</param>
/// <returns>An <see cref="IOrderedQueryable{T}"/> representing the ordered sequence.</returns>
IOrderedQueryable<T> Build(IQueryable<T> queryable);
}