using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Firenet
{
    public static class FireQueryExtensions
    {
        private static Query AddWhere(this Query query, string expr)
        {
            string[] splitado = Regex.Split(expr, "(\\.{1}|\\\"|(\\s))");

            return splitado switch
            {
                var e when e[3] == ">" => query.WhereGreaterThan(e[0], e[8]),
                var e when e[3] == ">=" => query.WhereGreaterThanOrEqualTo(e[0], e[8]),
                var e when e[3] == "<" => query.WhereLessThan(e[0], e[8]),
                var e when e[3] == "<=" => query.WhereLessThanOrEqualTo(e[0], e[8]),
                var e when e[3] == "==" => query.WhereEqualTo(e[0], e[8]),
                var e when e[3] == "!=" => query.WhereNotEqualTo(e[0], e[8]),
                var e when e[2].Contains("Start") => query.StartAt(),
                var e when e[2] == "Contains" => query
                    .WhereArrayContains(e[0], $"%{e[4]}%"),
                //.WhereGreaterThanOrEqualTo(e[0], e[4])
                //.WhereLessThanOrEqualTo(e[0], e[4] + '\uf8ff'),
                _ => throw new InvalidOperationException()
            };
        }

        public static async Task<IEnumerable<TEntity>> ToListAsync<TEntity>(this FireQuery<TEntity> firequery) where TEntity : class
        {
            return await Task.FromResult(firequery.ToList());
        }

        public static IEnumerable<TEntity> ToList<TEntity>(this FireQuery<TEntity> firequery) where TEntity : class
        {
            return firequery.Queries
                .SelectMany(q => q.GetSnapshot().Documents.Select(d => d.ConvertTo<TEntity>()))
                .Distinct(new EntityComparer<TEntity>())
                .ToList();
        }

        public static FireQuery<TEntity> Where<TEntity>(this FireQuery<TEntity> firequery, Expression<Func<TEntity, bool>> expression)
            where TEntity : class
        {
            List<List<string>> MapQueries(BinaryExpression exp)
            {
                if (exp.NodeType is ExpressionType.And or ExpressionType.AndAlso or ExpressionType.AndAssign)
                {
                    var left = MapQueries(exp.Left as BinaryExpression);
                    var right = MapQueries(exp.Right as BinaryExpression);
                    left.ForEach(l => right.ForEach(r => l.AddRange(r)));
                    return left;
                }
                if (exp.NodeType is ExpressionType.Or or ExpressionType.OrElse or ExpressionType.OrAssign)
                {
                    var left = MapQueries(exp.Left as BinaryExpression);
                    var right = MapQueries(exp.Right as BinaryExpression);
                    left.AddRange(right);
                    return left;
                }
                string parameter = expression.Parameters[0].Name;
                string stringExp = Regex.Replace(exp.ToString(), $@"\(|\)|{parameter}\.", string.Empty);
                return new List<List<string>>() { new List<string> { stringExp } };
            }

            var mappedQueries = MapQueries(expression.Body.Reduce() as BinaryExpression);

            foreach (var list in mappedQueries)
            {
                var quering = firequery.SourceQuery;
                list.ForEach(queryStr => quering = quering.AddWhere(queryStr));
                firequery.Queries.Add(quering);
            }

            return firequery;
        }
    }
}
