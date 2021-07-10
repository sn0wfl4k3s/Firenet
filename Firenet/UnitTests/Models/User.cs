using Google.Cloud.Firestore;
using System;

namespace UnitTests.Models
{
    [FirestoreData]
    public class User
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public string Email { get; set; }

        [FirestoreProperty]
        public string[] Ids { get; set; }

        [FirestoreProperty]
        public bool IsAdmin { get; set; }

        [FirestoreProperty]
        public int Points { get; set; }

        [FirestoreProperty]
        public DateTime? Release { get; set; }

        [FirestoreDocumentCreateTimestamp]
        public DateTime Created { get; set; }
    }
}
