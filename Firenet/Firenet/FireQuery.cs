using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Firenet
{
    public class FireQuery<TEntity> where TEntity : class
    {
        private readonly EntityComparer<TEntity> _comparer;
        private readonly Query _sourceQuery;

        public Query SourceQuery => _sourceQuery;

        public IList<Query> Queries { get; set; }

        public FireQuery(Query query)
        {
            _sourceQuery = query;
            _comparer = new EntityComparer<TEntity>();
            Queries = new List<Query>();
        }

        public async Task<TEntity[]> ToArrayAsync() => await Task.FromResult(ToArray());
        public async Task<IEnumerable<TEntity>> ToListAsync() => await Task.FromResult(ToList());

        public List<TEntity> ToList()
        {
            return Queries
                .SelectMany(q => q.GetSnapshot().Select(s => s.ConvertTo<TEntity>()))
                .Distinct(_comparer)
                .ToList();
        }

        public TEntity[] ToArray()
        {
            return Queries
                .SelectMany(q => q.GetSnapshot().Select(s => s.ConvertTo<TEntity>()))
                .Distinct(_comparer)
                .ToArray();
        }

        public FireQuery<TEntity> Where(Expression<Func<TEntity, bool>> expression)
        {
            List<List<string>> MapQueries(Expression exp)
            {
                var binary = exp as BinaryExpression;
                if (binary is not null)
                {
                    if (exp.NodeType is ExpressionType.And or ExpressionType.AndAlso or ExpressionType.AndAssign)
                    {
                        var left = MapQueries(binary.Left);
                        var right = MapQueries(binary.Right);
                        left.ForEach(l => right.ForEach(r => l.AddRange(r)));
                        return left;
                    }
                    if (exp.NodeType is ExpressionType.Or or ExpressionType.OrElse or ExpressionType.OrAssign)
                    {
                        var left = MapQueries(binary.Left);
                        var right = MapQueries(binary.Right);
                        left.AddRange(right);
                        return left;
                    }
                }
                string stringExp =
                    Regex.Replace(exp.ToString(), $@"\(|\)|{expression.Parameters[0].Name}\.", string.Empty);
                return new List<List<string>>() { new List<string> { stringExp } };
            }

            var entryExpression = expression.CanReduce ? expression.Body.Reduce() : expression.Body;

            var queries = MapQueries(entryExpression);

            foreach (var list in queries)
            {
                var quering = SourceQuery;
                list.ForEach(queryStr => quering = quering.AddQuery<TEntity>(queryStr));
                Queries.Add(quering);
            }

            return this;
        }
    }
}
