using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Firenet
{
    internal class SelectModifier : ExpressionVisitor
    {
        private readonly IDictionary<string, object> _values;

        public SelectModifier(IDictionary<string, object> values)
        {
            _values = values;
        }

        public Expression Modify(Expression expression)
        {
            return Visit(expression);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            object value = _values.FirstOrDefault(vs => vs.Key.Equals(node.Member.Name)).Value;

            return value switch
            {
                int => Expression.Constant((int)value),
                long => Expression.Constant((int)value),
                string => Expression.Constant((string)value),
                _ => Expression.Constant(value)
            };
        }
    }
}
