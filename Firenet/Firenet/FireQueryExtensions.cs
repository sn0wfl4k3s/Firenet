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
            string[] words = Regex.Split(expression.ToString(), @"\s|\{|\}|\.|\(|\)|\""|\'", RegexOptions.Compiled);

            PropertyInfo property = typeof(TEntity).GetProperties().FirstOrDefault(p => words.Contains(p.Name));

            BinaryExpression binary = expression as BinaryExpression;

            bool? isPropertyAtLeft = binary?.Left.ToString().Contains(property.Name);

            string method = (expression as MethodCallExpression)?.Method.Name;

            object value = (binary, isPropertyAtLeft, method, expression.NodeType) switch
            {
                (not null, false, _, _) => Expression.Lambda(binary.Left).Compile().DynamicInvoke(),
                (not null, true, _, _) => Expression.Lambda(binary.Right).Compile().DynamicInvoke(),
                (null, null, "Contains", _) => Regex.Replace(expression.ToString(), @".*?Contains\(\""|\""\)", string.Empty),
                (null, null, "StartsWith", _) => Regex.Replace(expression.ToString(), @".*?StartsWith\(\""|\""\)", string.Empty),
                (_, _, _, ExpressionType.Constant) => Expression.Lambda(expression).Compile().DynamicInvoke(),
                _ => null
            };

            if (property is not null && (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?)))
                value = DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc);

            return (expression.NodeType, value, method) switch
            {
                (ExpressionType.Equal, _, _) => query.WhereEqualTo(property.Name, value),
                (ExpressionType.NotEqual, _, _) => query.WhereNotEqualTo(property.Name, value),
                (ExpressionType.GreaterThan, _, _) => query.WhereGreaterThan(property.Name, value),
                (ExpressionType.GreaterThanOrEqual, _, _) => query.WhereGreaterThanOrEqualTo(property.Name, value),
                (ExpressionType.LessThanOrEqual, _, _) => query.WhereLessThanOrEqualTo(property.Name, value),
                (ExpressionType.LessThan, _, _) => query.WhereLessThan(property.Name, value),
                (ExpressionType.MemberAccess, null, _) => query.WhereEqualTo(property.Name, true),
                (ExpressionType.Not, null, _) => query.WhereEqualTo(property.Name, false),
                (ExpressionType.Call, not null and string, "Contains") => query.WhereArrayContains(property.Name, value),
                (ExpressionType.Call, not null and string, "StartsWith") => query
                    .WhereLessThanOrEqualTo(property.Name, $"{value}~")
                    .WhereGreaterThanOrEqualTo(property.Name, value),
                (ExpressionType.Constant, true, _) => query,
                (ExpressionType.Constant, false, _) => query.Limit(0),
                _ => throw new InvalidOperationException()
            };
        }
    }
}










