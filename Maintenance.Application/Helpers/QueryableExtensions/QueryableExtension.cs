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
        /// <summary>
        /// Used for paging. Can be used as an alternative to Skip(...).Take(...) chaining.
        /// </summary>
        public static IQueryable<T> PageBy<T>(this IQueryable<T> query, int skipCount, int maxResultCount)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            return query.Skip(skipCount).Take(maxResultCount);
        }

        /// <summary>
        /// Filters a <see cref="IQueryable{T}"/> by given predicate if given condition is true.
        /// </summary>
        /// <param name="query">Queryable to apply filtering</param>
        /// <param name="condition">A boolean value</param>
        /// <param name="predicate">Predicate to filter the query</param>
        /// <returns>Filtered or not filtered query based on <paramref name="condition"/></returns>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
        {
            return condition
                ? query.Where(predicate)
                : query;
        }

        /// <summary>
        /// Filters a <see cref="IQueryable{T}"/> by given predicate if given condition is true.
        /// </summary>
        /// <param name="query">Queryable to apply filtering</param>
        /// <param name="condition">A boolean value</param>
        /// <param name="predicate">Predicate to filter the query</param>
        /// <returns>Filtered or not filtered query based on <paramref name="condition"/></returns>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, int, bool>> predicate)
        {
            return condition
                ? query.Where(predicate)
                : query;
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string sortField, bool ascending = true)
        {
            var param = Expression.Parameter(typeof(T), "p");
            var parts = sortField.Split('.');
            var parent = parts.Aggregate<string, Expression>(param, Expression.Property);
            var conversion = Expression.Convert(parent, parent.Type);
            var exp = Expression.Lambda(conversion, param);
            var method = ascending ? "OrderBy" : "OrderByDescending";
            var types = new[] { query.ElementType, exp.Body.Type };
            var mce = Expression.Call(typeof(Queryable), method, types, query.Expression, exp);
            return query.Provider.CreateQuery<T>(mce);
        }

        public static IQueryable<T> If<T>(
        this IQueryable<T> query,
        bool should,
        params Func<IQueryable<T>, IQueryable<T>>[] transforms)
        {
            return should
                ? transforms.Aggregate(query,
                    (current, transform) => transform.Invoke(current))
                : query;
        }
    }
}
