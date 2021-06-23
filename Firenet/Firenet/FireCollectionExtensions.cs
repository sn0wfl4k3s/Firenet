using System;
using System.Linq;
using System.Reflection;

namespace Firenet
{
    internal static class FireCollectionExtensions
    {
        public static TEntity SetUtcDatetimes<TEntity>(this TEntity entity) where TEntity : class
        {
            entity.GetType()
                .GetRuntimeProperties()
                .Where(p => p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?))
                .Where(p => p.GetValue(entity) is not null)
                .Where(p => ((DateTime)p.GetValue(entity)).Kind != DateTimeKind.Utc)
                .ToList()
                .ForEach(p =>
                {
                    var value = (DateTime)entity.GetType().GetProperty(p.Name).GetValue(entity);
                    entity.GetType().GetProperty(p.Name).SetValue(entity, DateTime.SpecifyKind(value, DateTimeKind.Utc));
                });

            return entity;
        }
    }
}
