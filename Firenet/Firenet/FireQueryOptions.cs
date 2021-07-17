using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Firenet
{
    internal class FireQueryOptions : ICloneable
    {
        public string OrderByName { get; set; }
        public string OrderByDescendingName { get; set; }
        public SelectOptions SelectOptions { get; set; }
        public object Clone()
        {
            return this;
        }
    }

    internal class SelectOptions
    {
        public string[] Properties { get; set; }
        public Expression Expression { get; set; }
        public Type Before { get; set; }
        public Type After { get; set; }
    }
}