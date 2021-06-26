using Google.Cloud.Firestore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Firenet
{
    internal static class FireQueryExtensions
    {
        public static Query AddQuery<TEntity>(this Query query, Expression expression)
        {
            PropertyInfo prop = typeof(TEntity)
                .GetRuntimeProperties()
                .FirstOrDefault(p => expression.ToString().Contains(p.Name, StringComparison.InvariantCultureIgnoreCase));

            if (expression.ToString().Contains(".StartsWith("))
            {
                var valueString = Regex.Replace(expression.ToString(), @".*?StartsWith\(\""|\""\)", string.Empty);

                return query
                    .WhereLessThanOrEqualTo(prop.Name, $"{valueString}~")
                    .WhereGreaterThanOrEqualTo(prop.Name, valueString);
            }

            object value = (expression as BinaryExpression) switch
            {
                var b when b is not null && !b.Left.ToString().Contains(prop.Name)
                    => Expression.Lambda(b.Left).Compile().DynamicInvoke(),
                var b when b is not null && !b.Right.ToString().Contains(prop.Name)
                    => Expression.Lambda(b.Right).Compile().DynamicInvoke(),
                _ => null
            };

            if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
            {
                value = DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc);
            }

            return (node: expression.NodeType, value) switch
            {
                (ExpressionType.Equal, _) => query.WhereEqualTo(prop.Name, value),
                (ExpressionType.NotEqual, _) => query.WhereNotEqualTo(prop.Name, value),
                (ExpressionType.GreaterThan, _) => query.WhereGreaterThan(prop.Name, value),
                (ExpressionType.GreaterThanOrEqual, _) => query.WhereGreaterThanOrEqualTo(prop.Name, value),
                (ExpressionType.LessThanOrEqual, _) => query.WhereLessThanOrEqualTo(prop.Name, value),
                (ExpressionType.LessThan, _) => query.WhereLessThan(prop.Name, value),
                (ExpressionType.MemberAccess, null) => query.WhereEqualTo(prop.Name, true),
                (ExpressionType.Not, null) => query.WhereEqualTo(prop.Name, false),
                _ => throw new InvalidOperationException()
            };
        }
    }
}










