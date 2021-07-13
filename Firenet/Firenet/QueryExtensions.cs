using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Linq;

namespace Firenet
{
    public static class QueryExtensions
    {
        public static QuerySnapshot GetSnapshot(this Query collectionReference)
            => collectionReference.GetSnapshotAsync().GetAwaiter().GetResult();

        public static DocumentSnapshot GetSnapshot(this DocumentReference collectionReference)
            => collectionReference.GetSnapshotAsync().GetAwaiter().GetResult();

        public static IEnumerable<T> ToEnumerable<T>(this Query query) where T : class
            => query.GetSnapshot().Documents.AsParallel().Select(d => d.ConvertTo<T>());

        public static List<T> ToList<T>(this Query query) where T : class
            => query.ToEnumerable<T>().ToList();

        public static T[] ToArray<T>(this Query query) where T : class
            => query.ToEnumerable<T>().ToArray();

        public static T ToEnumerable<T>(this DocumentReference documents) where T : class
            => documents.GetSnapshot().ConvertTo<T>();
    }
}
