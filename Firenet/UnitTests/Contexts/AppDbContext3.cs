using Firenet;
using UnitTests.Models;

namespace UnitTests
{
    public class AppDbContext3 : FireContext
    {
        public IFireCollection<User> Users { get; set; }
    }
}
