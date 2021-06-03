using Google.Cloud.Firestore;

namespace UnitTests.Models
{
    [FirestoreData]
    public class Post
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty]
        public string Comment { get; set; }
    }
}
