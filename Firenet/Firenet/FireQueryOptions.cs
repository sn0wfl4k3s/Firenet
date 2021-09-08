using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Firenet
{
    internal class FireQueryOptions
    {
        public string OrderByName { get; set; }
        public string OrderByDescendingName { get; set; }
        public string[] Properties { get; set; }
        public List<SelectOptions> SelectOptions { get; set; } = new();
        public Type OriginType => SelectOptions[0].Before;
    }

    internal class SelectOptions
    {
        public LambdaExpression Expression { get; set; }
        public Type Before { get; set; }
        public Type After { get; set; }
    }
}