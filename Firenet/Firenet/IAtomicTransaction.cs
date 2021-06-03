using Google.Cloud.Firestore;
using System;
using System.Threading.Tasks;

namespace Firenet
{
    public interface IAtomicTransaction
    {
        /// <summary>
        /// Run a transaction for make atomic operations into this scope.
        /// </summary>
        /// <param name="callback">scope of the transaction must be run</param>
        /// <returns></returns>
        Task RunTransactionAsync(Func<Transaction, Task> callback);

        /// <summary>
        /// Run a transaction for make atomic operations into this scope.
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="callback">scope of the transaction must be run</param>
        /// <returns></returns>
        Task<R> RunTransactionAsync<R>(Func<Transaction, Task<R>> callback);
    }
}
