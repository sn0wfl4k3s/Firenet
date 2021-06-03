using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Firenet
{
    public interface IQuery<TEntity> where TEntity : class
    {
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
        /// Return all elements saved on the documents.
        /// </summary>
        /// <returns></returns>
        IEnumerable<TEntity> ToList();

        /// <summary>
        /// Return all elements saved on the documents asynchronously.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> ToListAsync();
    }
}
