using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Firenet
{
    public interface ICommandAsync<TEntity> where TEntity : class
    {
        /// <summary>
        /// Add asynchronously a new document that represent a entity model in the firestore collection.
        /// </summary>
        /// <param name="entity">Entity model</param>
        /// <param name="transaction">Atomic transaction</param>
        /// <returns></returns>
        Task<TEntity> AddAsync(TEntity entity, Transaction transaction = null);

        /// <summary>
        /// Update asynchronously a document that represent a entity model in the firestore collection.
        /// </summary>
        /// <param name="documentId">Document id of the entity</param>
        /// <param name="entity">Entity model</param>
        /// <param name="transaction">Atomic transaction</param>
        /// <returns></returns>
        Task<TEntity> UpdateAsync(string documentId, TEntity entity, Transaction transaction = null);

        /// <summary>
        /// Delete asynchronously a document that represent a entity model in the firestore collection.
        /// </summary>
        /// <param name="documentId">Document id of the entity</param>
        /// <param name="transaction">Atomic transaction</param>
        /// <returns></returns>
        Task DeleteAsync(string documentId, Transaction transaction = null);


        /// <summary>
        /// Add asynchronously new documents that represent entity models in the firestore collection.
        /// </summary>
        /// <param name="entities">Entity models</param>
        /// <param name="transaction">Atomic transaction</param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, Transaction transaction = null);

        /// <summary>
        /// Update asynchronously documents that represent entity models in the firestore collection.
        /// </summary>
        /// <param name="idsAndEntities">Dictionary of document id (key) and the entity model (value)</param>
        /// <param name="transaction">Atomic transaction</param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> UpdateRangeAsync(IDictionary<string, TEntity> idsAndEntities, Transaction transaction = null);

        /// <summary>
        /// Delete asynchronously documents that represent entity models in the firestore collection.
        /// </summary>
        /// <param name="documentsId">Collection of documents id</param>
        /// <param name="transaction">Atomic transaction</param>
        /// <returns></returns>
        Task DeleteRangeAsync(IEnumerable<string> documentsId, Transaction transaction = null);
    }
}
