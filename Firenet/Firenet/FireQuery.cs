using Google.Cloud.Firestore;
using System.Collections.Generic;

namespace Firenet
{
    public class FireQuery<T> where T : class
    {
        public Query SourceQuery { get; private set; }

        public IList<Query> Queries { get; private set; }

        public FireQuery(Query query)
        {
            SourceQuery = query;
            Queries = new List<Query>();
        }
    }
}
