using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Firenet
{
    public interface IQuery<TEntity> where TEntity : class
    {
        /// <summary>
        /// Contruct a query for your search with functions and expressions. 
        /// It's limited and experimental yet and be carefull into use this.
        /// </summary>
        /// <returns></returns>
        IFireQuery<TEntity> AsQueryable();

        /// <summary>
        /// Return the number of documents in collection.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Return the project Id.
        /// </summary>
        string ProjectId { get; }

        /// <summary>
        /// Contruct a query for your search.
        /// </summary>
        /// <returns></returns>
        Query Query();

        /// <summary>
        /// Return one entity with the id or path of the document.
        /// </summary>
        /// <param name="documentId">Id or Path of the document</param>
        /// <returns></returns>
        TEntity Find(string documentId);

        /// <summary>
        /// Return one entity with the id or path of the document asynchronously.
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        Task<TEntity> FindAsync(string documentId);

        /// <summary>
        /// Return all elements saved on the documents as enumerable of <typeparamref name="TEntity"/>.
        /// </summary>
        /// <returns></returns>
        IEnumerable<TEntity> ToEnumerable();

        /// <summary>
        /// Return all elements saved on the documents as a list of <typeparamref name="TEntity"/>.
        /// </summary>
        /// <returns></returns>
        List<TEntity> ToList();

        /// <summary>
        /// Return all elements saved on the documents as a list of <typeparamref name="TEntity"/> asynchronously.
        /// </summary>
        /// <returns></returns>
        Task<List<TEntity>> ToListAsync();

        /// <summary>
        /// Return all elements saved on the documents as a array of <typeparamref name="TEntity"/>.
        /// </summary>
        /// <returns></returns>
        TEntity[] ToArray();

        /// <summary>
        /// Return all elements saved on the documents as a array of <typeparamref name="TEntity"/> asynchronously.
        /// </summary>
        /// <returns></returns>
        Task<TEntity[]> ToArrayAsync();
    }
}
