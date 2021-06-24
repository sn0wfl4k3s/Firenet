using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Firenet
{
    public class FireQuery<TEntity> where TEntity : class
    {
        private readonly IEqualityComparer<TEntity> _comparer;
        private readonly Query _sourceQuery;

        private string _orderByPropertyName { get; set; }
        private IList<Query> _queries { get; set; }

        public FireQuery(Query query)
        {
            _sourceQuery = query;
            _comparer = new EntityComparer<TEntity>();
            _queries = new List<Query>();
            _orderByPropertyName = typeof(TEntity).GetProperties().Where(p => p.PropertyType == typeof(string)).ToArray()[1].Name;
        }

        public TEntity[] ToArray() => AsEnumerable().ToArray();
        public List<TEntity> ToList() => AsEnumerable().ToList();
        public async Task<TEntity[]> ToArrayAsync() => await Task.FromResult(ToArray());
        public async Task<IEnumerable<TEntity>> ToListAsync() => await Task.FromResult(ToList());

        public IEnumerable<TEntity> AsEnumerable()
        {
            return _queries
                .AsParallel()
                .SelectMany(q => q.GetSnapshot().Select(s => s.ConvertTo<TEntity>()))
                .AsSequential()
                .Distinct(_comparer)
                .OrderBy(e => e.GetType().GetProperty(_orderByPropertyName))
                .AsEnumerable();
        }


        public FireQuery<TEntity> OrderBy(Expression<Func<TEntity, object>> expression)
        {
            _orderByPropertyName = (expression.Body as MemberExpression).Member.Name;

            if (_queries.Count is 0)
            {
                _queries.Add(_sourceQuery);
            }

            _queries = _queries.Select(q => q.OrderBy(_orderByPropertyName)).ToList();

            return this;
        }

        public FireQuery<TEntity> Where(Expression<Func<TEntity, bool>> expression)
        {
            List<List<Expression>> MapQueries(Expression exp)
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

                return new List<List<Expression>>() { new List<Expression> { exp } };
            }

            var entryExpression = expression.CanReduce ? expression.Body.Reduce() : expression.Body;
            var expressionGroups = MapQueries(entryExpression);

            foreach (var expressions in expressionGroups)
            {
                var quering = _sourceQuery;
                expressions.ForEach(express => quering = quering.AddQuery<TEntity>(express));
                _queries.Add(quering);
            }

            return this;
        }
    }
}
