namespace SimplifiedDnd.DataBase.Abstractions;

public interface IOrderBuilder<T> {
  IOrderedQueryable<T> Build(IQueryable<T> queryable);
}