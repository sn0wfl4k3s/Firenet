using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Firenet
{
    public static class FireQueryExtensions
    {
        private static Query ToQuery(this Query query, string expr)
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
                var e when e[2] =="Contains" => query
                    .WhereArrayContains(e[0], $"%{e[4]}%"),
                //.WhereGreaterThanOrEqualTo(e[0], e[4])
                //.WhereLessThanOrEqualTo(e[0], e[4] + '\uf8ff'),
                _ => throw new InvalidOperationException()
            };
        }

        public static IEnumerable<TEntity> ToList<TEntity>(this FireQuery<TEntity> firequery) where TEntity : class
        {
            return firequery
                .Queries
                .SelectMany(q => q.GetSnapshot().Documents.Select(d => d.ConvertTo<TEntity>()))
                .Distinct(new EntityComparer<TEntity>())
                .ToList();
        }

        public static FireQuery<TEntity> Where<TEntity>(this FireQuery<TEntity> firequery, Expression<Func<TEntity, bool>> expression)
            where TEntity : class
        {
            string expBody = Regex.Replace(expression.Body.ToString(), @"^\(|\)$", string.Empty);

            string[] expressions =
                Regex.Split(expBody, @"\s(AndAlso|OrElse)\s")
                .Select(c => Regex.Replace(c, $@"\(|\)|{expression.Parameters[0].Name}\.", string.Empty))
                .ToArray();

            // https://regexr.com/5v74h
            if (expressions.Length == 1)
            {
                firequery.Queries.Add(firequery.SourceQuery.ToQuery(expressions[0]));
            }
            else
            {
                for (int i = 0; i < expressions.Length; ++i)
                {
                    if (expressions[i] == "AndAlso")
                    {
                        if (firequery.Queries.Count == 0)
                        {
                            var leftQuery = firequery.SourceQuery.ToQuery(expressions[i - 1]);
                            var rightQuery = leftQuery.ToQuery(expressions[i + 1]);
                            firequery.Queries.Add(rightQuery);
                        }
                        else
                        {
                            var leftQuery = firequery.Queries.Last();
                            var rightQuery = leftQuery.ToQuery(expressions[i + 1]);
                            firequery.Queries.Remove(leftQuery);
                            firequery.Queries.Add(rightQuery);
                        }
                    }
                    if (expressions[i] == "OrElse")
                    {
                        if (firequery.Queries.Count == 0)
                        {
                            var leftQuery = firequery.SourceQuery.ToQuery(expressions[i - 1]);
                            firequery.Queries.Add(leftQuery);
                        }
                        var rightQuery = firequery.SourceQuery.ToQuery(expressions[i + 1]);
                        firequery.Queries.Add(rightQuery);

                    }
                }
            }

            // https://regexr.com/5v285
            // https://regexr.com/5v2e8
            // (x.Id > 5) AndAlso (x.Warranty != False)
            //bool hasExpression(string expression) => Regex.IsMatch(expression, @"\(?.*?\)");
            //string[] getSheets(string expression) =>
            //    Regex.Match(expression, @"\((\w|\.|\d)*\s(>|>=|<|<=|!=|==)\s(\w|\.|\d)*\)").Captures
            //    .Select(c => c.Value)
            //    .ToArray();

            return firequery;
        }
    }
}
