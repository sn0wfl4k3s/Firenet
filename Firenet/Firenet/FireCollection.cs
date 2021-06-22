using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Firenet
{
    public class FireCollection<TEntity> : IFireCollection<TEntity> where TEntity : class
    {
        private readonly FirestoreDb _database;
        private readonly string _collectionName;

        public FireCollection(FirestoreDb database)
        {
            _database = database;
            _collectionName = $"{typeof(TEntity).Name}s";
        }
        public FireCollection(FirestoreDb database, string collectionName)
        {
            _database = database;
            _collectionName = collectionName;
        }

        #region Queries Implementation
        public virtual Query Query()
        {
            return _database.Collection(_collectionName);
        }

        public virtual FireQuery<TEntity> AsQueriable()
        {
            return new FireQuery<TEntity>(_database.Collection(_collectionName));
        }

        public virtual TEntity Find(string documentId)
        {
            return _database.Collection(_collectionName).Document(documentId).GetSnapshot().ConvertTo<TEntity>();
        }

        public virtual async Task<TEntity> FindAsync(string documentId) => await Task.FromResult(Find(documentId));

        public virtual IEnumerable<TEntity> ToEnumerable()
        {
            return _database.Collection(_collectionName).GetSnapshot().Documents.Select(d => d.ConvertTo<TEntity>());
        }

        public virtual TEntity[] ToArray() => ToEnumerable().ToArray();
        public virtual List<TEntity> ToList() => ToEnumerable().ToList();
        public virtual async Task<TEntity[]> ToArrayAsync() => await Task.FromResult(ToArray());
        public virtual async Task<List<TEntity>> ToListAsync() => await Task.FromResult(ToList());

        #endregion

        #region CommandSync Implementation
        public virtual TEntity Add(TEntity entity, Transaction transaction = null)
        {
            CollectionReference colRef = _database.Collection(_collectionName);
            if (transaction is not null)
            {
                DocumentReference docTransaction = colRef.Document();
                transaction.Create(docTransaction, entity);
                return entity;
            }
            DocumentReference doc = colRef.AddAsync(entity).GetAwaiter().GetResult();
            return doc.GetSnapshot().ConvertTo<TEntity>();
        }

        public virtual TEntity Update(string id, TEntity entity, Transaction transaction = null)
        {
            DocumentReference docReference = _database.Collection(_collectionName).Document(id);
            if (transaction is not null)
            {
                TEntity entityBefore = docReference.GetSnapshot().ConvertTo<TEntity>();
                ImmutableArray<(string name, object value)> propertiesBefore = entityBefore
                    .GetType()
                    .GetRuntimeProperties()
                    .Select(p => (name: p.Name, value: p.GetValue(entityBefore)))
                    .ToImmutableArray();
                IDictionary<string, object> propertiesToChange = entity
                    .GetType()
                    .GetRuntimeProperties()
                    .Select(p => (name: p.Name, value: p.GetValue(entity)))
                    .Where(t => !propertiesBefore.Contains(t))
                    .ToDictionary(d => d.name, d => d.value);
                transaction.Update(docReference, propertiesToChange);
                return entity;
            }
            docReference.SetAsync(entity, SetOptions.MergeAll);
            return entity;
        }

        public virtual void Delete(string id, Transaction transaction = null)
        {
            DocumentReference recordRef = _database.Collection(_collectionName).Document(id);
            if (transaction is not null)
            {
                transaction.Delete(recordRef);
                return;
            }
            recordRef.DeleteAsync().GetAwaiter().GetResult();
        }

        public virtual IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities, Transaction transaction = null)
        {
            return entities.AsParallel().Select(e => Add(e, transaction)).ToList();
        }

        public virtual IEnumerable<TEntity> UpdateRange(IDictionary<string, TEntity> idsAndEntities, Transaction transaction = null)
        {
            return idsAndEntities.Select(e => Update(e.Key, e.Value, transaction)).ToList();
        }

        public virtual void DeleteRange(IEnumerable<string> ids, Transaction transaction = null)
        {
            ids.ToList().ForEach(id => Delete(id, transaction));
        }
        #endregion

        #region CommandAsync Implementation
        public virtual async Task<TEntity> AddAsync(TEntity entity, Transaction transaction = null)
        {
            CollectionReference colRef = _database.Collection(_collectionName);
            if (transaction is not null)
            {
                DocumentReference docTransaction = colRef.Document();
                transaction.Create(docTransaction, entity);
                return entity;
            }
            DocumentReference doc = await colRef.AddAsync(entity);
            return doc.GetSnapshot().ConvertTo<TEntity>();
        }

        public virtual async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, Transaction transaction = null)
        {
            Task<TEntity>[] updateTasks = entities.AsParallel().Select(e => AddAsync(e, transaction)).ToArray();
            await Task.WhenAll(updateTasks);
            return updateTasks.Select(t => t.GetAwaiter().GetResult());
        }

        public virtual async Task<TEntity> UpdateAsync(string id, TEntity entity, Transaction transaction = null)
        {
            DocumentReference docReference = _database.Collection(_collectionName).Document(id);
            if (transaction is not null)
            {
                TEntity entityBefore = docReference.GetSnapshot().ConvertTo<TEntity>();
                ImmutableArray<(string name, object value)> propertiesBefore = entityBefore
                    .GetType()
                    .GetRuntimeProperties()
                    .Select(p => (name: p.Name, value: p.GetValue(entityBefore)))
                    .ToImmutableArray();
                IDictionary<string, object> propertiesToChange = entity
                    .GetType()
                    .GetRuntimeProperties()
                    .Select(p => (name: p.Name, value: p.GetValue(entity)))
                    .Where(t => !propertiesBefore.Contains(t))
                    .ToDictionary(d => d.name, d => d.value);
                transaction.Update(docReference, propertiesToChange);
                return entity;
            }
            await docReference.SetAsync(entity, SetOptions.MergeAll);
            return entity;
        }

        public virtual async Task<IEnumerable<TEntity>> UpdateRangeAsync(IDictionary<string, TEntity> entities, Transaction transaction = null)
        {
            Task<TEntity>[] updateTasks = entities.AsParallel().Select(e => UpdateAsync(e.Key, e.Value, transaction)).ToArray();
            await Task.WhenAll(updateTasks);
            return updateTasks.AsParallel().Select(t => t.GetAwaiter().GetResult());
        }

        public virtual async Task DeleteAsync(string id, Transaction transaction = null)
        {
            DocumentReference recordRef = _database.Collection(_collectionName).Document(id);
            if (transaction is not null)
            {
                transaction.Delete(recordRef);
                await Task.CompletedTask;
                return;
            }
            await recordRef.DeleteAsync();
            await Task.CompletedTask;
        }

        public async virtual Task DeleteRangeAsync(IEnumerable<string> ids, Transaction transaction = null)
        {
            Task[] updateTasks = ids.AsParallel().Select(id => DeleteAsync(id, transaction)).ToArray();
            await Task.WhenAll(updateTasks);
            await Task.CompletedTask;
        }
        #endregion
    }
}
