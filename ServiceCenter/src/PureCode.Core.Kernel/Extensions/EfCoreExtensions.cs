using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PureCode.Core.Extensions
{
  public static class EfCoreExtensions
  {
    public static string? ToSql<TEntity>(this IQueryable<TEntity> query) where TEntity : class
    {
      var enumerator = query.Provider.Execute<IEnumerable<TEntity>>(query.Expression).GetEnumerator();
      var relationalCommandCache = enumerator.Private("_relationalCommandCache");
      if (relationalCommandCache == null) return null;
      var selectExpression = relationalCommandCache.Private<SelectExpression>("_selectExpression");
      var factory = relationalCommandCache.Private<IQuerySqlGeneratorFactory>("_querySqlGeneratorFactory");

      if (factory == null) return null;
      var sqlGenerator = factory.Create();
      if (selectExpression == null) return null;
      var command = sqlGenerator.GetCommand(selectExpression);

      string sql = command.CommandText;
      return sql;
    }

    private static object? Private(this object obj, string privateField) => obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);

    private static T? Private<T>(this object obj, string privateField) => (T)obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj)!;

    public static async Task<(IEnumerable<TEntity> result, int totalCount, int totalPageCount)> PagedAsync<TEntity>(
      this IQueryable<TEntity> queryable, int page, int pageSize) where TEntity : class
    {
      if (pageSize < 1)
      {
        throw new ArgumentException($"{nameof(pageSize)} 不可以小于 1，现在是 {pageSize}");
      }

      int totalCount = await queryable.CountAsync();
      TEntity[]? list = await queryable.Skip(Math.Max(page - 1, 0) * pageSize).Take(pageSize).AsNoTracking()
        .ToArrayAsync();
      return (list, totalCount, (int)Math.Ceiling(totalCount / (float)pageSize));
    }

    public static async Task<(IEnumerable<TModel> result, int totalCount, int totalPageCount)> PagedAsync<TEntity,
      TModel>(
      this IQueryable<TEntity> queryable, Expression<Func<TEntity, TModel>> converter, int page, int pageSize)
      where TEntity : class
    {
      if (pageSize < 1)
      {
        throw new ArgumentException($"{nameof(pageSize)} 不可以小于 1，现在是 {pageSize}");
      }

      int totalCount = await queryable.CountAsync();
      TModel[]? list = await queryable.Skip(Math.Max(page - 1, 0) * pageSize).Take(pageSize).AsNoTracking()
        .Select(converter)
        .ToArrayAsync();
      return (list, totalCount, (int)Math.Ceiling(totalCount / (double)pageSize));
    }
  }
}