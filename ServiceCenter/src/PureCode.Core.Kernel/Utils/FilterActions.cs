using System.Linq.Expressions;

namespace PureCode.Core.Kernel.Utils
{
  public class FilterActions<T> : Dictionary<string, Func<string, Expression<Func<T, bool>>?>>
  {
    public IEnumerable<Expression<Func<T, bool>>> ToExpressions(Dictionary<string, string> filters)
    {
      List<Expression<Func<T, bool>>> list = [];
      foreach (KeyValuePair<string, string> kv in filters)
      {
        if (!TryGetValue(kv.Key, out Func<string, Expression<Func<T, bool>>?>? action))
        {
          continue;
        }

        Expression<Func<T, bool>>? expression = action.Invoke(kv.Value);
        if (expression != null)
        {
          list.Add(expression);
        }
      }

      return list;
    }

    public Expression<Func<T, bool>>? ToAndExpression(Dictionary<string, string> filters)
    {
      IEnumerable<Expression<Func<T, bool>>> expressions = ToExpressions(filters);
      return expressions.AndAll();
    }

    public Expression<Func<T, bool>>? ToOrExpression(Dictionary<string, string> filters)
    {
      IEnumerable<Expression<Func<T, bool>>> expressions = ToExpressions(filters);
      return expressions.OrAll();
    }
  }
}
