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

            Expression value = (expression as BinaryExpression) switch
            {
                var b when b != null && !b.Left.ToString().Contains(property.Name)
                    => b.Left,
                var b when b != null && !b.Right.ToString().Contains(property.Name)
                    => b.Right,
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
                string stringValue = string.Empty;

                if (stringExpression.Contains(".StartsWith("))
                {
                    stringValue = Regex.Replace(stringExpression, @".*?StartsWith\(\""|\""\)", string.Empty);

                    return query
                        .WhereLessThanOrEqualTo(property.Name, stringValue + '~')
                        .WhereGreaterThanOrEqualTo(property.Name, stringValue);
                }

                stringValue = Expression.Lambda(value).Compile().DynamicInvoke() as string;

                return expression.NodeType switch
                {
                    ExpressionType.Equal => query.WhereEqualTo(property.Name, stringValue),
                    ExpressionType.NotEqual => query.WhereNotEqualTo(property.Name, stringValue),
                    _ => throw new InvalidOperationException()
                };
            }

            if (double.TryParse(value.ToString(), out _))
            {
                double numericValue = double.Parse(value.ToString());

                return expression.NodeType switch
                {
                    ExpressionType.Equal => query.WhereEqualTo(property.Name, numericValue),
                    ExpressionType.NotEqual => query.WhereNotEqualTo(property.Name, numericValue),
                    ExpressionType.GreaterThan => query.WhereGreaterThan(property.Name, numericValue),
                    ExpressionType.GreaterThanOrEqual => query.WhereGreaterThanOrEqualTo(property.Name, numericValue),
                    ExpressionType.LessThanOrEqual => query.WhereLessThanOrEqualTo(property.Name, numericValue),
                    ExpressionType.LessThan => query.WhereLessThan(property.Name, numericValue),
                    _ => throw new InvalidOperationException()
                };
            }

            if (typeof(DateTime).Equals(property.PropertyType) || typeof(DateTime?).Equals(property.PropertyType))
            {
                DateTime datetimeValue = (DateTime)Expression.Lambda(value).Compile().DynamicInvoke();

                datetimeValue = DateTime.SpecifyKind(datetimeValue, DateTimeKind.Utc);

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










