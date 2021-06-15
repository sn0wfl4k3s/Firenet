using Google.Cloud.Firestore;
using System.Collections.Generic;

namespace Firenet
{
    public class FireQuery<T> where T : class
    {
        private readonly Query _sourceQuery;

        public Query SourceQuery => _sourceQuery;

        public IList<Query> Queries { get; set; } = new List<Query>();

        public FireQuery(Query query)
        {
            _sourceQuery = query;
        }


    }
}
