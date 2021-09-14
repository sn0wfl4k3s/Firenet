using Google.Cloud.Firestore;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Firenet
{   
    public abstract class FireContext : IAtomicTransaction, IDisposable
    {
        private FirestoreDb _firestoreDb;

        /// <summary>
        /// Instantiated the context with the environment variable 'GOOGLE_APPLICATION_CREDENTIALS' 
        /// which should previusly configured.
        /// </summary>
        protected FireContext()
        {
            Construct(OnConfiguring);
        }

        /// <summary>
        /// Instantiated the context with the <paramref name="configuring"/>.
        /// </summary>
        /// <param name="configuring"></param>
        protected FireContext(Action<FireOption> configuring)
        {
            Construct(configuring);
        }

        protected virtual void OnConfiguring(FireOption options) 
        {
            options.GetFromGoogleEnvironmentVariable();
        }

        #region AtomicTransaction Implementation
        public virtual async Task RunTransactionAsync(Func<Transaction, Task> callback)
            => await _firestoreDb.RunTransactionAsync(callback);

        public virtual async Task<R> RunTransactionAsync<R>(Func<Transaction, Task<R>> callback)
            => await _firestoreDb.RunTransactionAsync(callback);
        #endregion

        #region Private Essencials Methods
        private void Construct(Action<FireOption> configuring)
        {
            FireOption options = new();
            configuring.Invoke(options);
            _firestoreDb = InstantionFirestore(options);
            InstantiationCollections(_firestoreDb);
        }
        private static FirestoreDb InstantionFirestore(FireOption options)
        {
            FirestoreDbBuilder builder = new()
            {
                ProjectId = options.ProjectId,
                WarningLogger = options.WarningLogger,
                CredentialsPath = options.JsonCredentialsPath,
                ConverterRegistry = options.Converters,
            };

            return builder.Build();
        }
        private void InstantiationCollections(FirestoreDb firestoreDb)
        {
            GetType()
                .GetRuntimeProperties()
                .Where(p => p.GetValue(this) is null && Regex.IsMatch(p.PropertyType.FullName, @"Firenet\.?(IF|F)?ireCollection"))
                .Select(p =>
                {
                    string entityClassAssemblyName = Regex.Replace(p.PropertyType.AssemblyQualifiedName, @".*?\[\[| |\]\].*", string.Empty);
                    Type entityType = Type.GetType(entityClassAssemblyName);
                    Type collection = typeof(FireCollection<>).MakeGenericType(entityType);
                    string collectionName = GetType()
                        .GetProperty(p.Name)
                        .GetCustomAttributes(typeof(CollectionNameAttribute), false)
                        .FirstOrDefault() is not CollectionNameAttribute annotation ? p.Name : annotation.Name;
                    object instance = Activator.CreateInstance(collection, firestoreDb, collectionName);
                    return (name: p.Name, value: instance);
                })
                .ToList()
                .ForEach(p => GetType().GetProperty(p.name).SetValue(this, p.value));
        }
        #endregion

        public void Dispose() => GC.SuppressFinalize(this);
    }
}
