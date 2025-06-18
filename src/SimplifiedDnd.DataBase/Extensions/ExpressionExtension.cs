using System.Linq.Expressions;

namespace SimplifiedDnd.DataBase.Extensions;

internal static class ExpressionExtension {
  /// <summary>
/// Returns a predicate expression that always evaluates to true for any input of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the input parameter for the predicate.</typeparam>
/// <returns>An expression representing a predicate that always returns true.</returns>
internal static Expression<Func<T, bool>> True<T>() { return _ => true; }

  /// <summary>
  /// Combines two predicate expressions into a single expression representing their logical AND.
  /// </summary>
  /// <typeparam name="T">The type of the parameter in the predicate expressions.</typeparam>
  /// <param name="left">The first predicate expression.</param>
  /// <param name="right">The second predicate expression.</param>
  /// <returns>An expression that evaluates to true only if both input predicates are true for a given input.</returns>
  internal static Expression<Func<T, bool>> And<T>(
    this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right
  ) {
    InvocationExpression invokedExpression = Expression.Invoke(right, left.Parameters);
    return Expression.Lambda<Func<T, bool>>(
      Expression.AndAlso(left.Body, invokedExpression), left.Parameters);
  }
}