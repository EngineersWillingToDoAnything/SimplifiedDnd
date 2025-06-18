using System.Linq.Expressions;

namespace SimplifiedDnd.DataBase.Abstractions;

public interface IFilterBuilder<T> {
  Expression<Func<T, bool>> Build();
}