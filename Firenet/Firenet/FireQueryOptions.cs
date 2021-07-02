using System;

namespace Firenet
{
    internal class FireQueryOptions : ICloneable
    {
        public string OrderByName { get; set; }
        public string OrderByDescendingName { get; set; }

        public object Clone()
        {
            return this;
        }
    }
}
