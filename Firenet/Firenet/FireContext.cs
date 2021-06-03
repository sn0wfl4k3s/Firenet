using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Firenet
{
    public abstract class FireContext : IAtomicTransaction
    {
        protected abstract string JsonCredentials { get; }

        public FirestoreDb FirestoreDb { get; private set; }

        public FireContext()
        {
            FirestoreDb = LoadCredentials(JsonCredentials);
        }

        #region AtomicTransaction Implementation
        public virtual async Task RunTransactionAsync(Func<Transaction, Task> callback)
            => await FirestoreDb.RunTransactionAsync(callback);

        public virtual async Task<R> RunTransactionAsync<R>(Func<Transaction, Task<R>> callback)
            => await FirestoreDb.RunTransactionAsync(callback);
        #endregion

        private static FirestoreDb LoadCredentials(string jsonCredentialsPath)
        {
            string credentialsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, jsonCredentialsPath);
            var jsonObject = JsonConvert.DeserializeObject<CredentialFile>(File.ReadAllText(credentialsPath));
            var builder = new FirestoreDbBuilder { ProjectId = jsonObject.ProjectId, CredentialsPath = credentialsPath };
            return builder.Build();
        }
    }
}
