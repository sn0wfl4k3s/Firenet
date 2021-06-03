using Google.Cloud.Firestore;
using System.Collections.Generic;

namespace Firenet
{
    public interface ICommandSync<TEntity> where TEntity : class
    {
        /// <summary>
        /// Add a new document that represent a entity model in the firestore collection.
        /// </summary>
        /// <param name="entity">Entity model</param>
        /// <param name="transaction">Atomic transaction</param>
        /// <returns></returns>
        TEntity Add(TEntity entity, Transaction transaction = null);

        /// <summary>
        /// Update a document that represent a entity model in the firestore collection.
        /// </summary>
        /// <param name="documentId">Document id of the entity</param>
        /// <param name="entity">Entity model</param>
        /// <param name="transaction">Atomic transaction</param>
        /// <returns></returns>
        TEntity Update(string documentId, TEntity entity, Transaction transaction = null);

        /// <summary>
        /// Delete a document that represent a entity model in the firestore collection.
        /// </summary>
        /// <param name="documentId">Document id of the entity</param>
        /// <param name="transaction">Atomic transaction</param>
        /// <returns></returns>
        void Delete(string documentId, Transaction transaction = null);

        /// <summary>
        /// Add new documents that represent entity models in the firestore collection.
        /// </summary>
        /// <param name="entities">Entity models</param>
        /// <param name="transaction">Atomic transaction</param>
        /// <returns></returns>
        IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities, Transaction transaction = null);

        /// <summary>
        /// Update documents that represent entity models in the firestore collection.
        /// </summary>
        /// <param name="idsAndEntities">Dictionary of document id (key) and the entity model (value)</param>
        /// <param name="transaction">Atomic transaction</param>
        /// <returns></returns>
        IEnumerable<TEntity> UpdateRange(IDictionary<string, TEntity> idsAndEntities, Transaction transaction = null);

        /// <summary>
        /// Delete documents that represent entity models in the firestore collection.
        /// </summary>
        /// <param name="documentsId">Collection of documents id</param>
        /// <param name="transaction">Atomic transaction</param>
        /// <returns></returns>
        void DeleteRange(IEnumerable<string> documentsId, Transaction transaction = null);
    }
}
