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
        private string _orderByName;
        private string _orderByDescendingName;

        public FireQuery(Query query)
        {
            _sourceQuery = query;
            _comparer = new EntityComparer<TEntity>();
            _queries = new HashSet<Query>();
        }

        public TEntity First() => Take(1).ToArray().First();
        public TEntity First(Expression<Func<TEntity, bool>> expression) => Take(1).Where(expression).ToArray().First();

        public TEntity Last() => ToArray().Last();
        public TEntity Last(Expression<Func<TEntity, bool>> expression) => Where(expression).ToArray().Last();

        public TEntity[] ToArray() => AsEnumerable().ToArray();
        public List<TEntity> ToList() => AsEnumerable().ToList();

        public async Task<TEntity[]> ToArrayAsync() => await Task.FromResult(ToArray());
        public async Task<List<TEntity>> ToListAsync() => await Task.FromResult(ToList());

        public IEnumerable<TEntity> AsEnumerable()
        {
            if (_queries.Count is 0)
                _queries.Add(_sourceQuery);

            var queries = _queries
                .ToArray()
                .AsParallel()
                .SelectMany(q => q.GetSnapshot().Select(s => (created: s.CreateTime, entity: s.ConvertTo<TEntity>())));

            var results = (!string.IsNullOrEmpty(_orderByName), !string.IsNullOrEmpty(_orderByDescendingName)) switch
            {
                (true, false) => queries
                    .AsSequential()
                    .Select(t => t.entity)
                    .Distinct(_comparer)
                    .OrderBy(e => e.GetType().GetProperty(_orderByName)),
                (false, true) => queries
                    .AsSequential()
                    .Select(t => t.entity)
                    .Distinct(_comparer)
                    .OrderByDescending(e => e.GetType().GetProperty(_orderByDescendingName)),
                (false, false) => queries
                    .AsSequential()
                    .OrderBy(t => t.created)
                    .Select(t => t.entity)
                    .Distinct(_comparer),
                _ => throw new InvalidOperationException()
            };

            _orderByName = string.Empty;
            _orderByDescendingName = string.Empty;
            _queries.Clear();

            return results.AsEnumerable();
        }


        public FireQuery<TEntity> OrderBy(Expression<Func<TEntity, object>> expression)
        {
            _orderByDescendingName = string.Empty;
            _orderByName = typeof(TEntity)
                .GetProperties()
                .FirstOrDefault(p => expression.Body.ToString().Contains(p.Name))
                .Name;

            AddQueryToTheQueries(q => q.OrderBy(_orderByName));

            return this;
        }

        public FireQuery<TEntity> OrderByDescending(Expression<Func<TEntity, object>> expression)
        {
            _orderByName = string.Empty;
            _orderByDescendingName = typeof(TEntity)
                .GetProperties()
                .FirstOrDefault(p => expression.Body.ToString().Contains(p.Name))
                .Name;

            AddQueryToTheQueries(q => q.OrderByDescending(_orderByDescendingName));

            return this;
        }

        public FireQuery<TEntity> Take(int count)
        {
            AddQueryToTheQueries(q => q.Limit(count));

            return this;
        }

        public FireQuery<TEntity> TakeLast(int count)
        {
            AddQueryToTheQueries(q => q.LimitToLast(count));

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
