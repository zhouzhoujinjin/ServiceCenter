using System;
using System.Linq.Expressions;

namespace PureCode.Core.Kernel.Utils
{
  public static class ExpressionExtensions
  {
    /// <summary>
    /// 为表达式提供 And 运算
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="leftExpression"></param>
    /// <param name="rightExpression"></param>
    /// <returns></returns>
    public static Expression<Func<T, bool>>? And<T>(this Expression<Func<T, bool>>? leftExpression,
      Expression<Func<T, bool>>? rightExpression)
    {
      if (leftExpression == null)
      {
        return rightExpression;
      }

      if (rightExpression == null)
      {
        return leftExpression;
      }

      ParameterExpression paramExpr = Expression.Parameter(typeof(T));
      BinaryExpression exprBody = Expression.And(leftExpression.Body, rightExpression.Body);
      exprBody = (BinaryExpression)new ParameterReplacer(paramExpr).Visit(exprBody);

      return Expression.Lambda<Func<T, bool>>(exprBody, paramExpr);
    }

    public static Expression<Func<T, bool>>? AndAll<T>(this IEnumerable<Expression<Func<T, bool>>> list)
    {
      using IEnumerator<Expression<Func<T, bool>>> enumerator = list.GetEnumerator();
      if (!enumerator.MoveNext())
      {
        return null;
      }

      Expression<Func<T, bool>>? current = enumerator.Current;

      while (enumerator.MoveNext())
      {
        Expression<Func<T, bool>> right = enumerator.Current;
        current = current.And(right);
      }

      return current;
    }

    /// <summary>
    /// 为表达式提供 Or 运算
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="leftExpression"></param>
    /// <param name="rightExpression"></param>
    /// <returns></returns>
    public static Expression<Func<T, bool>>? Or<T>(this Expression<Func<T, bool>>? leftExpression,
      Expression<Func<T, bool>>? rightExpression)
    {
      if (leftExpression == null)
      {
        return rightExpression;
      }

      if (rightExpression == null)
      {
        return leftExpression;
      }

      ParameterExpression paramExpr = Expression.Parameter(typeof(T));
      BinaryExpression exprBody = Expression.Or(leftExpression.Body, rightExpression.Body);
      exprBody = (BinaryExpression)new ParameterReplacer(paramExpr).Visit(exprBody);

      return Expression.Lambda<Func<T, bool>>(exprBody, paramExpr);
    }

    public static Expression<Func<T, bool>>? OrAll<T>(this IEnumerable<Expression<Func<T, bool>>> list)
    {
      using IEnumerator<Expression<Func<T, bool>>> enumerator = list.GetEnumerator();
      if (!enumerator.MoveNext())
      {
        return null;
      }

      Expression<Func<T, bool>>? current = enumerator.Current;

      while (enumerator.MoveNext())
      {
        Expression<Func<T, bool>> right = enumerator.Current;
        current = current.Or(right);
      }

      return current;
    }
  }

  internal class ParameterReplacer : ExpressionVisitor
  {
    private readonly ParameterExpression _parameter;

    protected override Expression VisitParameter(ParameterExpression node)
    {
      return base.VisitParameter(_parameter);
    }

    internal ParameterReplacer(ParameterExpression parameter)
    {
      _parameter = parameter;
    }
  }
}