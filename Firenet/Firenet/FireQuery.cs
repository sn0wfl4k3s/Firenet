using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Firenet
{
    internal sealed class FireQuery<TEntity> : IFireQuery<TEntity> where TEntity : class
    {
        private readonly IEqualityComparer<DocumentSnapshot> _documentComparer;
        private readonly Query _sourceQuery;

        private HashSet<Query> _queries;
        private FireQueryOptions _options;

        public FireQuery(Query query)
        {
            _sourceQuery = query;
            _queries = new HashSet<Query>();
            _documentComparer = new DocumentSnapshotComparer();
            _options = new FireQueryOptions();
        }

        public int Count() => ToDocuments().Length;
        public int Count(Expression<Func<TEntity, bool>> expression) => Predicate(expression).ToDocuments().Length;
        public TEntity[] ToArray() => ToEnumerable().ToArray();
        public List<TEntity> ToList() => ToEnumerable().ToList();
        public bool Any(Expression<Func<TEntity, bool>> expression) => Predicate(expression).ToDocuments().Length > 0;
        public TEntity First(Expression<Func<TEntity, bool>> expression) => Predicate(expression).ToDocuments()[0].ConvertTo<TEntity>();
        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> expression)
        {
            DocumentSnapshot[] docs = Predicate(expression).ToDocuments();
            if (docs is null or { Length: 0 })
                return null;
            return docs[0].ConvertTo<TEntity>();
        }
        public TEntity Last(Expression<Func<TEntity, bool>> expression) => Predicate(expression).ToDocuments()[^1].ConvertTo<TEntity>();
        public TEntity LastOrDefault(Expression<Func<TEntity, bool>> expression)
        {
            DocumentSnapshot[] docs = Predicate(expression).ToDocuments();
            if (docs is null or { Length: 0 })
                return null;
            return docs[^1].ConvertTo<TEntity>();
        }

        public async Task<int> CountAsync() => await Task.FromResult(Count());
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> expression) => await Task.FromResult(Count(expression));
        public async Task<TEntity[]> ToArrayAsync() => await Task.FromResult(ToArray());
        public async Task<List<TEntity>> ToListAsync() => await Task.FromResult(ToList());
        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression) => await Task.FromResult(Any(expression));
        public async Task<TEntity> LastAsync(Expression<Func<TEntity, bool>> expression) => await Task.FromResult(Last(expression));
        public async Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> expression) => await Task.FromResult(First(expression));
        public async Task<TEntity> LastOrDefaultAsync(Expression<Func<TEntity, bool>> expression) => await Task.FromResult(LastOrDefault(expression));
        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> expression) => await Task.FromResult(FirstOrDefault(expression));

        private DocumentSnapshot[] ToDocuments()
        {
            if (_queries.Count is 0)
                _queries.Add(_sourceQuery);

            DocumentSnapshot[] documents = _queries
                .AsParallel()
                .SelectMany(q => q.GetSnapshot().Documents)
                .AsSequential()
                .Distinct(_documentComparer)
                .OrderBy(d => d.CreateTime.Value.ToDateTime().Ticks)
                .ToArray();

            _queries.Clear();

            return documents;
        }

        public IEnumerable<TEntity> ToEnumerable()
        {
            IEnumerable<TEntity> doc = ToDocuments().Select(d => d.ConvertTo<TEntity>());

            var options = _options.Clone() as FireQueryOptions;

            IEnumerable<TEntity> results = options switch
            {
                { OrderByName: not null, OrderByDescendingName: null } => doc
                    .OrderBy(e => e.GetType().GetProperty(options.OrderByName).GetValue(e, null)),
                { OrderByName: null, OrderByDescendingName: not null } => doc
                    .OrderByDescending(e => e.GetType().GetProperty(options.OrderByDescendingName).GetValue(e, null)),
                { OrderByName: not null, OrderByDescendingName: not null } => doc
                    .OrderBy(e => e.GetType().GetProperty(options.OrderByName).GetValue(e, null))
                    .OrderByDescending(e => e.GetType().GetProperty(options.OrderByDescendingName).GetValue(e, null)),
                _ => doc
            };

            _options = new FireQueryOptions();

            return results;
        }

        public IFireQuery<TEntity> OrderBy(Expression<Func<TEntity, object>> expression)
        {
            _options.OrderByName = typeof(TEntity)
                .GetProperties()
                .FirstOrDefault(p => expression.Body.ToString().Contains(p.Name))
                .Name;

            AddQueryToTheQueries(q => q.OrderBy(_options.OrderByName));

            return this;
        }

        public IFireQuery<TEntity> OrderByDescending(Expression<Func<TEntity, object>> expression)
        {
            _options.OrderByDescendingName = typeof(TEntity)
                .GetProperties()
                .FirstOrDefault(p => expression.Body.ToString().Contains(p.Name))
                .Name;

            AddQueryToTheQueries(q => q.OrderByDescending(_options.OrderByDescendingName));

            return this;
        }

        public IFireQuery<TEntity> Where(Expression<Func<TEntity, bool>> expression) => Predicate(expression);

        private FireQuery<TEntity> Predicate(Expression<Func<TEntity, bool>> expression)
        {
            Expression entryExpression = expression.CanReduce ? expression.Body.Reduce() : expression.Body;

            _queries = MapQueries(entryExpression)
                .AsParallel()
                .Select(egs => egs.Aggregate(_sourceQuery, (current, next) => current.AddQuery<TEntity>(next)))
                .ToHashSet();

            return this;
        }

        private IEnumerable<IEnumerable<Expression>> MapQueries(Expression expression)
        {
            BinaryExpression binary = expression as BinaryExpression;

            return (expression.NodeType, binary) switch
            {
                (ExpressionType.Or or ExpressionType.OrElse or ExpressionType.OrAssign, not null)
                    => MapQueries(binary.Left).Concat(MapQueries(binary.Right)),
                (ExpressionType.And or ExpressionType.AndAlso or ExpressionType.AndAssign, not null)
                    => MapQueries(binary.Left).SelectMany(l => MapQueries(binary.Right).Select(r => r.Concat(l))),
                _ => new Expression[][] { new[] { expression } }
            };
        }

        private void AddQueryToTheQueries(Func<Query, Query> function)
        {
            if (_queries.Count is 0)
                _queries.Add(_sourceQuery);

            _queries = _queries.Select(function).ToHashSet();
        }
    }
}
