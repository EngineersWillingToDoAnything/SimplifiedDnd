using System.Linq.Expressions;

namespace SimplifiedDnd.DataBase.Extensions;

internal static class ExpressionExtension {
  internal static Expression<Func<T, bool>> True<T>() { return _ => true; }

  internal static Expression<Func<T, bool>> And<T>(
    this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right
  ) {
    InvocationExpression invokedExpression = Expression.Invoke(right, left.Parameters);
    return Expression.Lambda<Func<T, bool>>(
      Expression.AndAlso(left.Body, invokedExpression), left.Parameters);
  }
}