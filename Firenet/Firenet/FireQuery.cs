﻿using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Firenet
{
    internal sealed class FireQuery<TEntity> : IFireQuery<TEntity>
    {
        private readonly IEqualityComparer<DocumentSnapshot> _documentComparer;
        private readonly Query _sourceQuery;

        private HashSet<Query> _queries;
        private FireQueryOptions _options;

        public FireQuery(Query query)
        {
            _sourceQuery = query;
            _queries = new HashSet<Query>();
            _options = new FireQueryOptions();
            _documentComparer = new DocumentSnapshotComparer();
        }

        public FireQuery(Query query, HashSet<Query> queries, FireQueryOptions options)
        {
            _sourceQuery = query;
            _queries = queries;
            _options = options;
            _documentComparer = new DocumentSnapshotComparer();
        }

        public int Count() => ToDocuments().Length;
        public int Count(Expression<Func<TEntity, bool>> expression) => Predicate(expression).ToDocuments().Length;
        public long LongCount() => ToDocuments().LongLength;
        public long LongCount(Expression<Func<TEntity, bool>> expression) => Predicate(expression).ToDocuments().LongLength;
        public TEntity[] ToArray() => ToEnumerable().ToArray();
        public List<TEntity> ToList() => ToEnumerable().ToList();
        public bool All() => Count() == Count();
        public bool All(Expression<Func<TEntity, bool>> expression) => Count(expression) == Count();
        public bool Any() => Count() > 0;
        public bool Any(Expression<Func<TEntity, bool>> expression) => Count(expression) > 0;
        public TEntity First() => ToDocuments()[0].ConvertTo<TEntity>();
        public TEntity First(Expression<Func<TEntity, bool>> expression) => Predicate(expression).ToDocuments()[0].ConvertTo<TEntity>();
        public TEntity FirstOrDefault()
        {
            DocumentSnapshot[] docs = ToDocuments();
            if (docs is null or { Length: 0 }) return default;
            return docs[0].ConvertTo<TEntity>();
        }
        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> expression)
        {
            DocumentSnapshot[] docs = Predicate(expression).ToDocuments();
            if (docs is null or { Length: 0 }) return default;
            return docs[0].ConvertTo<TEntity>();
        }
        public TEntity Last() => ToDocuments()[^1].ConvertTo<TEntity>();
        public TEntity Last(Expression<Func<TEntity, bool>> expression) => Predicate(expression).ToDocuments()[^1].ConvertTo<TEntity>();
        public TEntity LastOrDefault()
        {
            DocumentSnapshot[] docs = ToDocuments();
            if (docs is null or { Length: 0 }) return default;
            return docs[^1].ConvertTo<TEntity>();
        }
        public TEntity LastOrDefault(Expression<Func<TEntity, bool>> expression)
        {
            DocumentSnapshot[] docs = Predicate(expression).ToDocuments();
            if (docs is null or { Length: 0 }) return default;
            return docs[^1].ConvertTo<TEntity>();
        }

        public async Task<int> CountAsync() => await Task.FromResult(Count());
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> expression) => await Task.FromResult(Count(expression));
        public async Task<long> LongCountAsync() => await Task.FromResult(LongCount());
        public async Task<long> LongCountAsync(Expression<Func<TEntity, bool>> expression) => await Task.FromResult(LongCount(expression));
        public async Task<TEntity[]> ToArrayAsync() => await Task.FromResult(ToArray());
        public async Task<List<TEntity>> ToListAsync() => await Task.FromResult(ToList());
        public async Task<bool> AllAsync() => await Task.FromResult(All());
        public async Task<bool> AllAsync(Expression<Func<TEntity, bool>> expression) => await Task.FromResult(All(expression));
        public async Task<bool> AnyAsync() => await Task.FromResult(Any());
        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression) => await Task.FromResult(Any(expression));
        public async Task<TEntity> LastAsync() => await Task.FromResult(Last());
        public async Task<TEntity> LastAsync(Expression<Func<TEntity, bool>> expression) => await Task.FromResult(Last(expression));
        public async Task<TEntity> FirstAsync() => await Task.FromResult(First());
        public async Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> expression) => await Task.FromResult(First(expression));
        public async Task<TEntity> LastOrDefaultAsync() => await Task.FromResult(LastOrDefault());
        public async Task<TEntity> LastOrDefaultAsync(Expression<Func<TEntity, bool>> expression) => await Task.FromResult(LastOrDefault(expression));
        public async Task<TEntity> FirstOrDefaultAsync() => await Task.FromResult(FirstOrDefault());
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

        public IFireQuery<TResult> Select<TResult>(Expression<Func<TEntity, TResult>> expression) where TResult : notnull
        {
            if (_options.SelectOptions.Count is 0)
            {
                _options.Properties = typeof(TEntity)
                        .GetProperties()
                        .Where(p => expression.Body.ToString().Contains(p.Name))
                        .Select(p => p?.GetCustomAttribute<FirestorePropertyAttribute>()?.Name ?? p.Name)
                        .ToArray();
                if (_queries.Count is 0) _queries.Add(_sourceQuery);
                _queries = _queries.Select(q => q.Select(_options.Properties)).ToHashSet();
            }
            SelectOptions selectOptions = new() { Before = typeof(TEntity), After = typeof(TResult), Expression = expression };
            _options.SelectOptions.Add(selectOptions);
            return new FireQuery<TResult>(_sourceQuery, _queries, _options);
        }

        public IEnumerable<TEntity> ToEnumerable()
        {
            IEnumerable<TEntity> entities;
            if (typeof(TEntity) != _options.OriginType)
            {
                MethodInfo convertTo = typeof(DocumentSnapshot)
                    .GetMethod("ConvertTo")
                    .MakeGenericMethod(_options.OriginType);
                entities = ToDocuments()
                    .Select(d => convertTo.Invoke(d, null))
                    .Select(e => (TEntity)_options.SelectOptions.Aggregate(e, (p, select) => select.Expression.Compile().DynamicInvoke(p)));
            }
            else
                entities = ToDocuments().Select(d => d.ConvertTo<TEntity>());

            IEnumerable<TEntity> filtered = _options switch
            {
                { OrderByName: not null, OrderByDescendingName: null } => entities
                    .OrderBy(e => e.GetType().GetProperty(_options.OrderByName).GetValue(e, null)),
                { OrderByName: null, OrderByDescendingName: not null } => entities
                    .OrderByDescending(e => e.GetType().GetProperty(_options.OrderByDescendingName).GetValue(e, null)),
                { OrderByName: not null, OrderByDescendingName: not null } => entities
                    .OrderBy(e => e.GetType().GetProperty(_options.OrderByName).GetValue(e, null))
                    .OrderByDescending(e => e.GetType().GetProperty(_options.OrderByDescendingName).GetValue(e, null)),
                _ => entities
            };

            return filtered;
        }

        public IFireQuery<TEntity> OrderBy<TKey>(Expression<Func<TEntity, TKey>> expression) where TKey : notnull
        {
            _options.OrderByName = typeof(TEntity)
                .GetProperties()
                .FirstOrDefault(p => expression.Body.ToString().Contains(p.Name))
                .Name;

            AddQueryToTheQueries(q => q.OrderBy(_options.OrderByName));

            return this;
        }

        public IFireQuery<TEntity> OrderByDescending<TKey>(Expression<Func<TEntity, TKey>> expression) where TKey : notnull
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
