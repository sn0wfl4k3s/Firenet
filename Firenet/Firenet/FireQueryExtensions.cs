using Google.Cloud.Firestore;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Firenet
{
    internal static class FireQueryExtensions
    {
        public static Query AddQuery<TEntity>(this Query query, string stringExpression)
        {
            string[] splitted = Regex.Split(stringExpression, "(\\.{1}|\\\"|(\\s))");

            PropertyInfo property = typeof(TEntity)
                .GetRuntimeProperties()
                .FirstOrDefault(p => stringExpression.Contains(p.Name, StringComparison.InvariantCultureIgnoreCase));

            return splitted switch
            {
                var e when typeof(bool).Equals(property.PropertyType) && splitted.Length == 1
                    => query.WhereEqualTo(property.Name, !stringExpression.Contains("Not")),
                var e when "==".Equals(e[3]) && typeof(string).Equals(property.PropertyType) => query.WhereEqualTo(e[0], e[8]),
                var e when "==".Equals(e[3]) && typeof(bool).Equals(property.PropertyType)
                    && (bool.TrueString.Equals(e[6]) || bool.FalseString.Equals(e[6]))
                    => query.WhereEqualTo(e[0], bool.Parse(e[6])),
                var e when "==".Equals(e[3]) => query.WhereEqualTo(e[0], double.Parse(e[6])),
                var e when "!=".Equals(e[3]) && typeof(string).Equals(property.PropertyType) => query.WhereNotEqualTo(e[0], e[8]),
                var e when "!=".Equals(e[3]) && typeof(bool).Equals(property.PropertyType)
                    && (bool.TrueString.Equals(e[6]) || bool.FalseString.Equals(e[6]))
                    => query.WhereNotEqualTo(e[0], bool.Parse(e[6])),
                var e when "!=".Equals(e[3]) => query.WhereNotEqualTo(e[0], double.Parse(e[6])),
                var e when "<".Equals(e[3]) => query.WhereLessThan(e[0], double.Parse(e[6])),
                var e when ">".Equals(e[3]) => query.WhereGreaterThan(e[0], double.Parse(e[6])),
                var e when "<=".Equals(e[3]) => query.WhereLessThanOrEqualTo(e[0], double.Parse(e[6])),
                var e when ">=".Equals(e[3]) => query.WhereGreaterThanOrEqualTo(e[0], double.Parse(e[6])),
                var e when "StartsWith".Equals(e[2]) => query
                    .WhereLessThanOrEqualTo(e[0], e[4] + '~')
                    .WhereGreaterThanOrEqualTo(e[0], e[4]),
                _ => throw new InvalidOperationException($"Operation '{stringExpression}' is not supported.")
            };
        }

    }
}










