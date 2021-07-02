using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Firenet
{
    public abstract class FireContext : IAtomicTransaction, IDisposable
    {
        /// <summary>
        /// Json credential path of your firebase admin
        /// </summary>
        protected abstract string JsonCredentials { get; }

        public FirestoreDb FirestoreDb { get; private set; }

        public FireContext()
        {
            FirestoreDb = LoadFirestoreDb(JsonCredentials);
        }

        #region AtomicTransaction Implementation
        public virtual async Task RunTransactionAsync(Func<Transaction, Task> callback)
            => await FirestoreDb.RunTransactionAsync(callback);

        public virtual async Task<R> RunTransactionAsync<R>(Func<Transaction, Task<R>> callback)
            => await FirestoreDb.RunTransactionAsync(callback);
        #endregion

        private static FirestoreDb LoadFirestoreDb(string jsonCredentialsPath)
        {
            if (string.IsNullOrEmpty(jsonCredentialsPath))
                throw new ArgumentException(nameof(JsonCredentials));
            if (!File.Exists(jsonCredentialsPath))
                throw new FileNotFoundException(nameof(JsonCredentials));
            var jsonObject = JsonConvert.DeserializeObject<CredentialFile>(File.ReadAllText(jsonCredentialsPath));
            var builder = new FirestoreDbBuilder { ProjectId = jsonObject.ProjectId, CredentialsPath = jsonCredentialsPath };
            return builder.Build();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
