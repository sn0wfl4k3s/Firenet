using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Firenet
{
    public static class FireContextBuilder<TContext> where TContext : FireContext
    {
        public static TContext Build()
        {
            TContext context = Activator.CreateInstance<TContext>();

            context.GetType()
                .GetRuntimeProperties()
                .Where(p => p.GetValue(context) is null && Regex.IsMatch(p.PropertyType.FullName, @"Firenet\.?(IF|F)?ireCollection"))
                .Select(p =>
                {
                    string entityClassAssemblyName = Regex
                        .Replace(p.PropertyType.AssemblyQualifiedName, @".*?\[\[| |\]\].*", string.Empty);
                    Type entityType = Type.GetType(entityClassAssemblyName);
                    Type collection = typeof(FireCollection<>).MakeGenericType(entityType);
                    string collectionName = context
                        .GetType()
                        .GetProperty(p.Name)
                        .GetCustomAttributes(typeof(CollectionNameAttribute), false)
                        .FirstOrDefault() is not CollectionNameAttribute annotation ? p.Name : annotation.Name;
                    object instance = Activator.CreateInstance(collection, context.FirestoreDb, collectionName);
                    return (name: p.Name, value: instance);
                })
                .ToList()
                .ForEach(p => context.GetType().GetProperty(p.name).SetValue(context, p.value));

            return context;
        }
    }
}
