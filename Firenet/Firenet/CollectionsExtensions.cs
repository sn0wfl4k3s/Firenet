using System.Collections.Generic;
using System.Linq;

namespace Firenet
{
    public static class CollectionsExtensions
    {
        public static (T Head, IEnumerable<T> Tail) HeadAndTail<T>(this IEnumerable<T> collection)
        {
            return (collection.First(), collection.Skip(1));
        }
    }
}
