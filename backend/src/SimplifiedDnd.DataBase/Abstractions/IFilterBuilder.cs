using System.Linq.Expressions;

namespace SimplifiedDnd.DataBase.Abstractions;

internal interface IFilterBuilder<T> {
  Expression<Func<T, bool>> Build();
}