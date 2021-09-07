using Firenet;
using UnitTests.Models;

namespace UnitTests
{
    public class AppDbContext : FireContext
    {
        public IFireCollection<User> Users { get; set; }
        public IFireCollection<Post> Posts { get; set; }
    }
}
