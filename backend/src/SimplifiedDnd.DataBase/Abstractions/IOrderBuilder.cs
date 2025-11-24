namespace SimplifiedDnd.DataBase.Abstractions;

internal interface IOrderBuilder<T> {
  IOrderedQueryable<T> Build(IQueryable<T> queryable);
}