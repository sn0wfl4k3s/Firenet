using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Firenet
{
    public sealed class FireQuery<TEntity> where TEntity : class
    {
        private readonly IEqualityComparer<TEntity> _comparer;
        private readonly Query _sourceQuery;

        private HashSet<Query> _queries;
        private FireQueryOptions _options;

        public FireQuery(Query query)
        {
            _sourceQuery = query;
            _queries = new HashSet<Query>();
            _comparer = new EntityComparer<TEntity>();
            _options = new FireQueryOptions();
        }

        public bool Any(Expression<Func<TEntity, bool>> expression) => Where(expression).ToEnumerable().Any(expression.Compile());
        public TEntity Last(Expression<Func<TEntity, bool>> expression) => Where(expression).ToEnumerable().Last();
        public TEntity First(Expression<Func<TEntity, bool>> expression) => Where(expression).ToEnumerable().First();
        public TEntity[] ToArray() => ToEnumerable().ToArray();
        public List<TEntity> ToList() => ToEnumerable().ToList();

        public async Task<TEntity> LastAsync(Expression<Func<TEntity, bool>> expression) => await Task.FromResult(Last(expression));
        public async Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> expression) => await Task.FromResult(First(expression));
        public async Task<TEntity[]> ToArrayAsync() => await Task.FromResult(ToArray());
        public async Task<List<TEntity>> ToListAsync() => await Task.FromResult(ToList());
        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression) => await Task.FromResult(Any(expression));

        public IEnumerable<TEntity> ToEnumerable()
        {
            if (_queries.Count is 0)
                _queries.Add(_sourceQuery);

            var options = _options.Clone() as FireQueryOptions;

            var results = _options switch
            {
                { OrderByName: not null, OrderByDescendingName: null } => _queries
                    .ToArray()
                    .AsParallel()
                    .SelectMany(q => q.GetSnapshot().Select(s => s.ConvertTo<TEntity>()))
                    .AsSequential()
                    .Distinct(_comparer)
                    .OrderBy(e => e.GetType().GetProperty(options.OrderByName).Name),
                { OrderByName: null, OrderByDescendingName: not null } => _queries
                    .ToArray()
                    .AsParallel()
                    .SelectMany(q => q.GetSnapshot().Select(s => s.ConvertTo<TEntity>()))
                    .AsSequential()
                    .Distinct(_comparer)
                    .OrderByDescending(e => e.GetType().GetProperty(options.OrderByDescendingName)),
                { OrderByName: not null, OrderByDescendingName: not null } => _queries
                    .ToArray()
                    .AsParallel()
                    .SelectMany(q => q.GetSnapshot().Select(s => s.ConvertTo<TEntity>()))
                    .AsSequential()
                    .Distinct(_comparer)
                    .OrderBy(e => e.GetType().GetProperty(options.OrderByName))
                    .OrderByDescending(e => e.GetType().GetProperty(options.OrderByDescendingName)),
                _ => _queries
                    .ToArray()
                    .AsParallel()
                    .SelectMany(q => q.GetSnapshot().Select(s => (created: s.CreateTime, entity: s.ConvertTo<TEntity>())))
                    .AsSequential()
                    .OrderBy(t => t.created)
                    .Select(t => t.entity)
                    .Distinct(_comparer)
            };

            _queries.Clear();
            _options = new FireQueryOptions();

            return results;
        }


        public FireQuery<TEntity> OrderBy(Expression<Func<TEntity, object>> expression)
        {
            _options.OrderByName = typeof(TEntity)
                .GetProperties()
                .FirstOrDefault(p => expression.Body.ToString().Contains(p.Name))
                .Name;

            AddQueryToTheQueries(q => q.OrderBy(_options.OrderByName));

            return this;
        }

        public FireQuery<TEntity> OrderByDescending(Expression<Func<TEntity, object>> expression)
        {
            _options.OrderByDescendingName = typeof(TEntity)
                .GetProperties()
                .FirstOrDefault(p => expression.Body.ToString().Contains(p.Name))
                .Name;

            AddQueryToTheQueries(q => q.OrderByDescending(_options.OrderByDescendingName));

            return this;
        }

        public FireQuery<TEntity> Where(Expression<Func<TEntity, bool>> expression)
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
            var binary = expression as BinaryExpression;

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
