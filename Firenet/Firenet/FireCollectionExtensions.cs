using System;
using System.Linq;
using System.Reflection;

namespace Firenet
{
    internal static class FireCollectionExtensions
    {
        public static TEntity SetUtcDatetimes<TEntity>(this TEntity entity)
            where TEntity : class
        {
            bool ValidProperty(PropertyInfo p) =>
                (p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?)) &&
                p.GetValue(entity) is not null &&
                ((DateTime)p.GetValue(entity)).Kind != DateTimeKind.Utc;

            //entity
            //    .GetType()
            //    .GetRuntimeProperties()
            //    .AsParallel()
            //    .Where(ValidProperty)
            //    .ForAll(p =>
            //    {
            //        var value = (DateTime)p.GetValue(entity);
            //        p.SetValue(entity, DateTime.SpecifyKind(value, DateTimeKind.Utc));
            //    });

            return entity;
        }
    }
}
