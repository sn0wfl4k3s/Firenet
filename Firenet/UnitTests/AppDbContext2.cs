using Firenet;
using Google.Cloud.Firestore;
using System;

namespace UnitTests
{
    public enum Type
    {
        Admin,
        Default
    }

    [FirestoreData]
    public class User2
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty]
        public Guid Hash { get; set; }

        [FirestoreProperty]
        public Type Type { get; set; }
    }

    public class AppDbContext2 : FireContext
    {
        public AppDbContext2(FirestoreDb firestoreDb) : base(firestoreDb)
        {
        }

        [CollectionName("Others")]
        public IFireCollection<User2> Users { get; set; }
    }
}
