using Google.Cloud.Firestore;
using System;
using System.Threading.Tasks;

namespace Firenet
{
    public abstract class FireContext : IAtomicTransaction, IDisposable
    {
        public FirestoreDb FirestoreDb { get; private set; }

        public FireContext(FirestoreDb firestoreDb)
        {
            FirestoreDb = firestoreDb;
        }

        #region AtomicTransaction Implementation
        public virtual async Task RunTransactionAsync(Func<Transaction, Task> callback)
            => await FirestoreDb.RunTransactionAsync(callback);

        public virtual async Task<R> RunTransactionAsync<R>(Func<Transaction, Task<R>> callback)
            => await FirestoreDb.RunTransactionAsync(callback);
        #endregion

        public void Dispose() => GC.SuppressFinalize(this);
    }
}
