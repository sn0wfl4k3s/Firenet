using Google.Cloud.Firestore;

namespace Firenet
{
    public static class QueryExtensions
    {
        public static QuerySnapshot GetSnapshot(this Query collectionReference)
        {
            return collectionReference.GetSnapshotAsync().GetAwaiter().GetResult();
        }
        public static DocumentSnapshot GetSnapshot(this DocumentReference collectionReference)
        {
            return collectionReference.GetSnapshotAsync().GetAwaiter().GetResult();
        }
    }
}
