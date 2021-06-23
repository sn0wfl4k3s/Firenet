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
            string stringExpression = expression.ToString();

            PropertyInfo property = typeof(TEntity)
                .GetRuntimeProperties()
                .FirstOrDefault(p => stringExpression.Contains(p.Name, StringComparison.InvariantCultureIgnoreCase));

            var binary = expression as BinaryExpression;

            string value = binary switch
            {
                var b when b != null && !b.Left.ToString().Contains(property.Name)
                    => b.Left.ToString().Replace("\"", string.Empty),
                var b when b != null && !b.Right.ToString().Contains(property.Name)
                    => b.Right.ToString().Replace("\"", string.Empty),
                _ => null
            };

            if (typeof(bool).Equals(property.PropertyType))
            {
                return expression.NodeType switch
                {
                    ExpressionType.Not => query.WhereEqualTo(property.Name, false),
                    ExpressionType.MemberAccess => query.WhereEqualTo(property.Name, true),
                    ExpressionType.NotEqual => query.WhereEqualTo(property.Name, stringExpression.Contains(bool.TrueString)),
                    ExpressionType.Equal => query.WhereEqualTo(property.Name, stringExpression.Contains(bool.TrueString)),
                    _ => throw new InvalidOperationException()
                };
            }

            if (typeof(string).Equals(property.PropertyType))
            {
                if (stringExpression.Contains(".StartsWith("))
                {
                    value = Regex.Replace(stringExpression, @".*?StartsWith\(\""|\""\)", string.Empty);
                    return query
                        .WhereLessThanOrEqualTo(property.Name, value + '~')
                        .WhereGreaterThanOrEqualTo(property.Name, value);
                }

                return expression.NodeType switch
                {
                    ExpressionType.Equal => query.WhereEqualTo(property.Name, value),
                    ExpressionType.NotEqual => query.WhereNotEqualTo(property.Name, value),
                    _ => throw new InvalidOperationException()
                };
            }

            if (double.TryParse(value, out _))
            {
                return expression.NodeType switch
                {
                    ExpressionType.Equal => query.WhereEqualTo(property.Name, double.Parse(value)),
                    ExpressionType.NotEqual => query.WhereNotEqualTo(property.Name, double.Parse(value)),
                    ExpressionType.GreaterThan => query.WhereGreaterThan(property.Name, double.Parse(value)),
                    ExpressionType.GreaterThanOrEqual => query.WhereGreaterThanOrEqualTo(property.Name, double.Parse(value)),
                    ExpressionType.LessThanOrEqual => query.WhereLessThanOrEqualTo(property.Name, double.Parse(value)),
                    ExpressionType.LessThan => query.WhereLessThan(property.Name, double.Parse(value)),
                    _ => throw new InvalidOperationException()
                };
            }

            if (typeof(DateTime).Equals(property.PropertyType) || typeof(DateTime?).Equals(property.PropertyType))
            {
                var datetimeValue = DateTime.SpecifyKind(Convert.ToDateTime(value), DateTimeKind.Utc);

                return expression.NodeType switch
                {
                    ExpressionType.Equal => query.WhereEqualTo(property.Name, datetimeValue),
                    ExpressionType.NotEqual => query.WhereNotEqualTo(property.Name, datetimeValue),
                    ExpressionType.GreaterThan => query.WhereGreaterThan(property.Name, datetimeValue),
                    ExpressionType.GreaterThanOrEqual => query.WhereGreaterThanOrEqualTo(property.Name, datetimeValue),
                    ExpressionType.LessThanOrEqual => query.WhereLessThanOrEqualTo(property.Name, datetimeValue),
                    ExpressionType.LessThan => query.WhereLessThan(property.Name, datetimeValue),
                    _ => throw new InvalidOperationException()
                };
            }

            throw new InvalidOperationException();
        }

    }
}










