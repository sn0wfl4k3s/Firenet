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
        private string _orderByPropertyName;
        private string _orderByDescendingPropertyName;

        public FireQuery(Query query)
        {
            _sourceQuery = query;
            _comparer = new EntityComparer<TEntity>();
            _queries = new HashSet<Query>();
        }

        public TEntity[] ToArray() => AsEnumerable().ToArray();
        public List<TEntity> ToList() => AsEnumerable().ToList();
        public async Task<TEntity[]> ToArrayAsync() => await Task.FromResult(ToArray());
        public async Task<IEnumerable<TEntity>> ToListAsync() => await Task.FromResult(ToList());

        public IEnumerable<TEntity> AsEnumerable()
        {
            if (_queries.Count is 0)
                _queries.Add(_sourceQuery);

            var queries = _queries
                .ToArray()
                .AsParallel()
                .SelectMany(q => q.GetSnapshot().Select(s => (created: s.CreateTime, entity: s.ConvertTo<TEntity>())));

            _queries.Clear();

            if (!string.IsNullOrEmpty(_orderByPropertyName))
            {
                return queries
                    .AsSequential()
                    .Select(t => t.entity)
                    .Distinct(_comparer)
                    .OrderBy(e => e.GetType().GetProperty(_orderByPropertyName));
            }

            if (!string.IsNullOrEmpty(_orderByDescendingPropertyName))
            {
                return queries
                    .AsSequential()
                    .Select(t => t.entity)
                    .Distinct(_comparer)
                    .OrderByDescending(e => e.GetType().GetProperty(_orderByDescendingPropertyName));
            }

            return queries
                .AsSequential()
                .OrderBy(t => t.created)
                .Select(t => t.entity)
                .Distinct(_comparer);
        }


        public FireQuery<TEntity> OrderBy(Expression<Func<TEntity, object>> expression)
        {
            _orderByDescendingPropertyName = string.Empty;
            _orderByPropertyName = typeof(TEntity)
                .GetProperties()
                .FirstOrDefault(p => expression.Body.ToString().Contains(p.Name))
                .Name;

            if (_queries.Count is 0)
                _queries.Add(_sourceQuery);

            _queries = _queries.Select(q => q.OrderBy(_orderByPropertyName)).ToHashSet();

            return this;
        }

        public FireQuery<TEntity> OrderByDescending(Expression<Func<TEntity, object>> expression)
        {
            _orderByPropertyName = string.Empty;
            _orderByDescendingPropertyName = typeof(TEntity)
                .GetProperties()
                .FirstOrDefault(p => expression.Body.ToString().Contains(p.Name))
                .Name;

            if (_queries.Count is 0)
                _queries.Add(_sourceQuery);

            _queries = _queries.Select(q => q.OrderByDescending(_orderByDescendingPropertyName)).ToHashSet();

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
    }
}
