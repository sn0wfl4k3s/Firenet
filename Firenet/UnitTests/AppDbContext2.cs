using Firenet;
using Google.Cloud.Firestore;
using System;

namespace UnitTests
{

    public class GuidConverter : IFirestoreConverter<Guid>
    {
        public Guid FromFirestore(object value) => Guid.Parse(value.ToString());
        public object ToFirestore(Guid value) => value.ToString();
    }

    [FirestoreData]
    public class User2
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty]
        public Guid Hash { get; set; }
    }

    public class AppDbContext2 : FireContext
    {
        [CollectionName("Others")]
        public IFireCollection<User2> Users { get; set; }
    }
}
