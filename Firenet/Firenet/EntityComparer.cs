using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Firenet
{
    internal class EntityComparer<TEntity> : IEqualityComparer<TEntity>
    {
        public bool Equals(TEntity x, TEntity y)
        {
            return x.GetHashCode().Equals(y.GetHashCode());
        }

        public int GetHashCode([DisallowNull] TEntity obj)
        {
            return JsonConvert.SerializeObject(obj).GetHashCode();
        }
    }
}
