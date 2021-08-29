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

            object value = (binary, isPropertyAtLeft, method) switch
            {
                (not null, false, _) => Expression.Lambda(binary.Left).Compile().DynamicInvoke(),
                (not null, true, _) => Expression.Lambda(binary.Right).Compile().DynamicInvoke(),
                (null, null, "Contains") => Regex.Replace(expression.ToString(), @".*?Contains\(\""|\""\)", string.Empty),
                (null, null, "StartsWith") => Regex.Replace(expression.ToString(), @".*?StartsWith\(\""|\""\)", string.Empty),
                _ => null
            };

            if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                value = DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc);

            string propertyName = property.Name;
            var attribute = property.GetCustomAttribute<FirestorePropertyAttribute>();
            if (attribute != null)
            {
                propertyName = attribute.Name;
            }

            return (expression.NodeType, value, method) switch
            {
                (ExpressionType.Equal, _, _) => query.WhereEqualTo(propertyName, value),
                (ExpressionType.NotEqual, _, _) => query.WhereNotEqualTo(propertyName, value),
                (ExpressionType.GreaterThan, _, _) => query.WhereGreaterThan(propertyName, value),
                (ExpressionType.GreaterThanOrEqual, _, _) => query.WhereGreaterThanOrEqualTo(propertyName, value),
                (ExpressionType.LessThanOrEqual, _, _) => query.WhereLessThanOrEqualTo(propertyName, value),
                (ExpressionType.LessThan, _, _) => query.WhereLessThan(propertyName, value),
                (ExpressionType.MemberAccess, null, _) => query.WhereEqualTo(propertyName, true),
                (ExpressionType.Not, null, _) => query.WhereEqualTo(propertyName, false),
                (ExpressionType.Call, not null and string, "Contains") => query.WhereArrayContains(propertyName, value),
                (ExpressionType.Call, not null and string, "StartsWith") => query
                    .WhereLessThanOrEqualTo(propertyName, $"{value}~")
                    .WhereGreaterThanOrEqualTo(propertyName, value),
                _ => throw new InvalidOperationException()
            };
        }
    }
}










