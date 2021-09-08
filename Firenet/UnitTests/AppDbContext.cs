using Firenet;
using Google.Cloud.Firestore;
using UnitTests.Models;

namespace UnitTests
{
    public class AppDbContext : FireContext
    {
        public AppDbContext(FirestoreDb firestoreDb) : base(firestoreDb)
        {
        }

        public IFireCollection<User> Users { get; set; }
        public IFireCollection<Post> Posts { get; set; }
    }
}
