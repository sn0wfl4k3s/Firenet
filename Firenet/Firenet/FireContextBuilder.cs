using Google.Cloud.Firestore;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Firenet
{
    public static class FireContextBuilder<TContext> where TContext : FireContext
    {
        /// <summary>
        /// Build with the <paramref name="options"/>.
        /// </summary>
        /// <returns></returns>
        public static TContext Build(Action<FireOption> options)
        {
            FireOption fireOption = new();
            options.Invoke(fireOption);
            FirestoreDbBuilder builder = new() { ProjectId = fireOption.ProjectId, CredentialsPath = fireOption.JsonCredentialsPath };
            FirestoreDb firestoreDb = builder.Build();
            return CreateInstance(firestoreDb);
        }

        /// <summary>
        /// Build with the environment variable 'GOOGLE_APPLICATION_CREDENTIALS' which should previusly configured.
        /// </summary>
        /// <returns></returns>
        public static TContext Build()
        {
            FireOption options = new();
            options.GetFromGoogleEnvironmentVariable();
            FirestoreDbBuilder builder = new() { ProjectId = options.ProjectId, CredentialsPath = options.JsonCredentialsPath };
            FirestoreDb firestoreDb = builder.Build();
            return CreateInstance(firestoreDb);
        }

        private static TContext CreateInstance(FirestoreDb firestoreDb)
        {
            TContext context = Activator.CreateInstance(typeof(TContext), firestoreDb) as TContext;
            context.GetType()
                .GetRuntimeProperties()
                .Where(p => p.GetValue(context) is null && Regex.IsMatch(p.PropertyType.FullName, @"Firenet\.?(IF|F)?ireCollection"))
                .Select(p =>
                {
                    string entityClassAssemblyName = Regex.Replace(p.PropertyType.AssemblyQualifiedName, @".*?\[\[| |\]\].*", string.Empty);
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
