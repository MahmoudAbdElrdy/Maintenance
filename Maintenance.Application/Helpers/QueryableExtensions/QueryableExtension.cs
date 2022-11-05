using System;
using System.Linq;
using System.Linq.Expressions;

namespace Maintenance.Application.Helpers.QueryableExtensions
{
    static class QueryableExtension
    {
        public static IOrderedQueryable<T> AppendOrderByAscending<T, TKey>(this IQueryable<T> source, Expression<Func<T, TKey>> sortSelector)
        {
            return (source as IOrderedQueryable<T>)?.ThenBy(sortSelector)
                ?? source.OrderBy(sortSelector);
        }

        public static IOrderedQueryable<T> AppendOrderByDescending<T, TKey>(this IQueryable<T> source, Expression<Func<T, TKey>> sortSelector)
        {
            return (source as IOrderedQueryable<T>)?.ThenByDescending(sortSelector)
                ?? source.OrderByDescending(sortSelector);
        }
    }
}
