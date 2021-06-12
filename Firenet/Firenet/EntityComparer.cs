using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Firenet
{
    class EntityComparer<TEntity> : IEqualityComparer<TEntity>
    {
        public bool Equals(TEntity x, TEntity y)
        {
            return JsonConvert.SerializeObject(x).Equals(JsonConvert.SerializeObject(y));
        }

        public int GetHashCode([DisallowNull] TEntity obj)
        {
            return JsonConvert.SerializeObject(obj).GetHashCode();
        }
    }
}
